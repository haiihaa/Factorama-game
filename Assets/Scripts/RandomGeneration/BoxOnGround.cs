using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxOnGround : MonoBehaviour
{
    public int width, height, scale;

    public float threshold, secondLay;

    public Vector3 origin; // bottom left for grid

    public GameObject boxPrefab;

    // Start is called before the first frame update
    void Start()
    {
        int xOffset = Random.Range(0, 11);
        int yOffset = Random.Range(0, 11);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float xCoord = (float)i / width * scale + xOffset;
                float yCoord = (float)j / height * scale + yOffset;

                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                if (sample > threshold)
                {
                    // create object 
                    GameObject item = GameObject.Instantiate(boxPrefab);
                    item.transform.position = origin + new Vector3(i, 0, j);

                    item.transform.localScale *= Random.Range(0.7f, 1.5f);
                    item.transform.Rotate(0.0f, Random.Range(-45f, 45f), 0.0f, Space.Self);

                    // add box on top of it
                    if (sample > secondLay)
                    {
                        GameObject itemOnTop = GameObject.Instantiate(boxPrefab);
                        itemOnTop.transform.position = origin + new Vector3(i, 1, j);

                        itemOnTop.transform.localScale = item.transform.localScale * Random.Range(0.5f, 1.0f);
                        itemOnTop.transform.Rotate(0.0f, Random.Range(-45f, 45f), 0.0f, Space.Self);
                    }

                }

            }
        }
    }

    // split area into grid, randomly pick grid to allocate item


}
