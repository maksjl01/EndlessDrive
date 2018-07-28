using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class Meteors : MonoBehaviour {

    public METEORS[] meteors;
    public GameObject player;
    float meteorSpread;
    public float meteorSpreadMultiplier = 1;
    public float startingChance = 0.1f;
    public float chanceIncrease = 0.1f;
    float chance;
    public float meteorStartHeight = 15;
    public float fallDelay = 0.5f;
    public float fallDelayDecrease = 0.1f;
    [Space]
    public GameObject marker;

    Queue<MeteorThreadInfo> meteorThreadQueue = new Queue<MeteorThreadInfo>();

    private void OnEnable()
    {
        StartCoroutine(WaitThenStart());
        MapGenerator mapGen = FindObjectOfType<MapGenerator>();
        meteorSpread = mapGen.mapChunkSize * meteorSpreadMultiplier;
        chance = startingChance;
        fallDelay = 0.4f;
    }

    IEnumerator WaitThenStart()
    {
        yield return new WaitForSeconds(2);
        StartThread();
    }

    public void StartThread()
    {
        ThreadStart threadStart = new ThreadStart(delegate {
            FindPositions();
        });
        Thread thread = new Thread(threadStart);
        thread.Start();
    }

    void FindPositions()
    {
        List<Vector3> positions = new List<Vector3>();

        System.Random rand = new System.Random();
        for (float x = -meteorSpread/2; x < meteorSpread/2; x++)
        {
            for (float y = -meteorSpread/2; y < meteorSpread/2; y++)
            {
                if (rand.NextDouble() < chance)
                    positions.Add(new Vector3(x, 0, y));
            }
        }
        lock (meteorThreadQueue)
        {
            meteorThreadQueue.Enqueue(new MeteorThreadInfo(MeteorFall, positions));
        }
    }

    void MeteorFall(List<Vector3> _positions)
    {
        if(fallDelay > 0)
            fallDelay -= fallDelayDecrease;
        chance += chanceIncrease;
        if(meteorSpread > 10)
            meteorSpread -= 1f;

        StartCoroutine(Instantiate(_positions, fallDelay));
    }

    IEnumerator Instantiate(List<Vector3> _positions, float fallDelay)
    {
        List<Vector3> pos = _positions;
        while(pos.Count > 0)
        {
            int r = UnityEngine.Random.Range(0, pos.Count);
            float loop = UnityEngine.Random.Range(0.0f, 1.0f);
            float temp = 0;
            int a = 0;
            for (int i = 0; i < meteors.Length; i++)
            {
                temp += meteors[i].chance;
                if (loop < temp)
                {
                    a = i;
                    break;
                }
            }

            GameObject inst = Instantiate(meteors[a].gameobject);

            inst.transform.position = new Vector3(player.transform.position.x, 0, player.transform.position.z) + new Vector3(pos[r].x, meteorStartHeight, pos[r].z);
            Meteor m = inst.AddComponent<Meteor>();
            m.Mass = meteors[a].mass;
            m.Damage = meteors[a].damage;
            m.marker = marker;
            m.transform.localScale = m.transform.localScale * meteors[a].size;
            m.transform.localRotation = (meteors[a].randomRotation) ? Quaternion.Euler(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)) : Quaternion.identity;

            pos.RemoveAt(r);
            yield return new WaitForSeconds(fallDelay);
        }

        //maybe add some offset on player position
        print("new thread");
        StartThread();
    }

    private void Update()
    {
        if(meteorThreadQueue.Count > 0)
        {
            MeteorThreadInfo i = meteorThreadQueue.Dequeue();
            i.callback(i.positions);
        }
    }

    private struct MeteorThreadInfo
    {
        public List<Vector3> positions;
        public Action<List<Vector3>> callback;

        public MeteorThreadInfo(Action<List<Vector3>> action, List<Vector3> _positions)
        {
            this.positions = _positions;
            this.callback = action;
        }
    }

	[System.Serializable]
    public class METEORS
    {
        public GameObject gameobject;
        [Range(0, 1)]
        public float chance;
        public float size;
        public float mass;
        public float damage;
        public bool randomSize;
        public bool randomRotation;
    }
}
