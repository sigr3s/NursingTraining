using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT
{
    public class TileMap : MonoBehaviour {
        
        public List<List<MapTile>> map = new List<List<MapTile>>();
        public Material defaultTileMaterial;
        public Material wallMaterial;



        public int xSize = 20;
        public int zSize = 20;

        public void Initialize(int xSize, int zSize){
            this.xSize = xSize;
            this.zSize = zSize;

            for(int i = 0; i < xSize; i++){
                float posx = (float) (i - xSize/2.0f);

                GameObject xContainer = new GameObject(i.ToString());
                xContainer.transform.parent = this.transform;

                xContainer.transform.localPosition = new Vector3(posx, 0 , 0);

                List<MapTile> column = new List<MapTile>();

                for(int j =0; j < zSize; j++){
                    float posz = (float) (j - zSize/2.0f);

                    var mapTileGamObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    mapTileGamObject.transform.parent = xContainer.transform;
                    mapTileGamObject.transform.localPosition = new Vector3(0,0,posz);

                    if(defaultTileMaterial != null){
                        mapTileGamObject.GetComponent<Renderer>().material = defaultTileMaterial;
                    }

                    MapTile mapTile = mapTileGamObject.AddComponent<MapTile>();
                    mapTile.id = (i*xSize + j);
                    column.Add(mapTile);
                }
            }

            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);            
            wall.transform.localScale = new Vector3(xSize, 3 , 0.005f);
            wall.transform.position = new Vector3(-0.5f  ,   1.5f ,  zSize/2.0f - 0.5f);
            wall.name = "wall";

            wall.transform.parent = this.transform;

            var wall2 = GameObject.CreatePrimitive(PrimitiveType.Cube);            
            wall2.transform.localScale = new Vector3(0.005f, 3 , xSize);
            wall2.transform.position = new Vector3( -xSize/2.0f - 0.5f  ,   1.5f ,  -0.5f);
            wall2.name = "wall";

            wall2.transform.parent = this.transform;


            if(wallMaterial != null){
                wall.GetComponent<Renderer>().material = wallMaterial;
                wall2.GetComponent<Renderer>().material = wallMaterial;
            }

        }
    }
}