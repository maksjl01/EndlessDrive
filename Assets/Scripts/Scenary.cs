using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class Scenary : MonoBehaviour{

    public float density = 0.1f;
    public float coinChance = 0.1f;
    public ASSETS[] assets;
    public CoinClass coin;

    List<Vector3> boolList = new List<Vector3>();

    Queue threadQueue = new Queue();

    public IEnumerator Threading(GameObject obj)
    {
        yield return new WaitForSeconds(1f);
        Vector3[] vertices = obj.GetComponent<MeshFilter>().mesh.vertices;
        ThreadStart threadStart = new ThreadStart(delegate { Make(vertices.Length, vertices, obj); });
        Thread thread = new Thread(threadStart);
        thread.Start();
    }

    void Make(int len, Vector3[] vertices, GameObject game)
    {
        Dictionary<int, Vector3> posList = new Dictionary<int, Vector3>();

        boolList.Clear();

        System.Random rand = new System.Random();
        for (int i = 0; i < vertices.Length; i++)
        {
            if (rand.NextDouble() < (density))
                posList.Add(i, vertices[i]);
        }

        lock (threadQueue)
        {
            threadQueue.Enqueue(new ThreadInformation(Place, vertices, game, posList));
        }
    }

    void Place(Vector3[] vertices, GameObject game, Dictionary<int, Vector3> _posList)
    {
        foreach(KeyValuePair<int, Vector3> pair in _posList)
        {
            if (UnityEngine.Random.Range(0.0f, 1.0f) < coinChance)
            {
                GameObject coinO = Instantiate(
                    coin.coinObject, 
                    game.transform.TransformPoint(pair.Value) + new Vector3(0, 0.8f), 
                    Quaternion.identity, game.transform);
                coinO.transform.localScale = coinO.transform.localScale * coin.size;

                Coin c = coinO.AddComponent<Coin>();
                c.SpinSpeed = coin.spinSpeed;
                c.SpinDirection = coin.spinDirection;
                c.OssilationHeight = coin.osillationHeight;
                c.OssilationStartHeight = coin.osillationStartHeight;
            }
            else
            {
                GameObject inst;
                int r = UnityEngine.Random.Range(0, assets.Length);

                inst = Instantiate(assets[r].gameobject, game.transform.TransformPoint(pair.Value), Quaternion.identity, game.transform) as GameObject;
                inst.transform.localRotation = Quaternion.FromToRotation(inst.transform.up, game.GetComponent<MeshFilter>().mesh.normals[pair.Key]);

                if (!assets[r].randomSize)
                    inst.transform.localScale *= assets[r].size;
                else
                    inst.transform.localScale *= UnityEngine.Random.Range((assets[r].size / 2), (assets[r].size * 1.6f));

                if (assets[r].matchTerrainCol)
                {
                    if (inst.transform.childCount > 0)
                    {
                        foreach (MeshRenderer rend in inst.GetComponentsInChildren<MeshRenderer>())
                        {
                            rend.material.color = Terrain_Flat.instance.Colors[UnityEngine.Random.Range(0, Terrain_Flat.instance.BaseColorCount)];
                        }
                    }
                }
            }
        }
    }

    private void Update()
    {
        if(threadQueue.Count > 0)
        {
            ThreadInformation TI = (ThreadInformation)threadQueue.Dequeue();
            TI.action(TI.vertices, TI.game, TI.posList);
            EndlessTerrain.startChunkCountSetup += 1;
            if (EndlessTerrain.startChunkCountSetup > EndlessTerrain.instance.chunkInviewDST)
                GameManager.instance.ReadyScene();
        }
    }

    public struct ThreadInformation
    {
        public Action<Vector3[], GameObject, Dictionary<int, Vector3>> action;
        public Vector3[] vertices;
        public GameObject game;
        public Dictionary<int, Vector3> posList;
        public ThreadInformation(Action<Vector3[], GameObject, Dictionary<int, Vector3>> _action, Vector3[] _vertics, GameObject _game,Dictionary<int, Vector3> _posList)
        {
            action = _action;
            vertices = _vertics;
            game = _game;
            posList = _posList;
        }
    }

    [System.Serializable]
    public class ASSETS
    {
        public GameObject gameobject;
        public float size;
        public bool matchTerrainCol;
        public bool randomSize;
        public float chance;
    }

    [System.Serializable]
    public class CoinClass
    {
        public GameObject coinObject;
        public float spinSpeed;
        public int spinDirection;
        public float osillationHeight;
        public float osillationStartHeight;
        public float size;
    }
}
