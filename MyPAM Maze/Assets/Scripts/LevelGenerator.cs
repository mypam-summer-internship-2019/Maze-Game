using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    //These textures are created in Piskel, the online sprite editor. They are saved as PNGs, then in Import settings they are set to Read/Write, no compression, no filtering
    public Texture2D map;

    public ColourToPrefab[] colourMappings;

    public GameObject wall_I;
    public GameObject wall_T;
    public GameObject wall_X;
    public GameObject wall_L;
    public GameObject wall_end;
    public GameObject wall_o;
    public GameObject clone;

    void Start()
    {
        GenerateLevel();
    }

    /*
     * This goes through every pixel in the png file and generates a tile for each one.
    */
    void GenerateLevel()
    {
        for (int x = 0; x < map.width; x++) 
        {
            for (int y = 0; y < map.height; y++)
            {
                GenerateTile(x, y);
            }
        }
    }

    /*
     * This gets the colour of the pixel at the current coordinates and matches it to a prefab, then instantiates that prefab at the corresponding world coordinates.
     * It also handles extra features like requesting the correct wall prefab and orientation are used, and assigning the green tile to the start position.
    */
    void GenerateTile(int x, int y)
    {
        Color pixelColour = map.GetPixel(x, y);
        if (pixelColour.a > 0.5)
        {
            pixelColour.a = 1;
        }
        else
        {        
            // The pixel is transparent so ignore it. 
            // Outside the boundaries of the image do not count as transparent.
            return;
        }
        
        foreach (ColourToPrefab colourMapping in colourMappings)
        {
            // Iterate through each stored colour mapping and if a colour matches the current pixel colour then...
            if (colourMapping.colour.Equals(pixelColour))
            {
                // If the pixel is completely black then it is a wall, which needs to be handled specially due to the various shapes possible.
                if (pixelColour == new Color(0, 0, 0, 1))
                {
                   // Debug.Log(pixelColour);

                    GetWallPrefab(x, y, colourMapping);
                }
                // Otherwise it is a regular ColourToPrefab object which has the prefab stored in ColourMapping.
                else
                {
                    Instantiate(colourMapping.prefab, new Vector3(x * 2, .01f, y * 2), Quaternion.Euler(new Vector3(0, 0, 0)));
                }
               
            }
        }
    }

    /*
     * This IF/ELSE structure goes through every relevant combination of adjacent grid positions and determines the shape and rotation of the wall,
     * which then instantiates the correct prefab with the correct orientation and world position.
     * GetPixel(0,0) is the bottom right, so y+1 is up a pixel and x+1 is right a pixel.
     * For help with navigating the structure, see 'Wall Shape Map' image within the scripts folder.
     * I can not find a more programmatic solution, and if I could I still think this is easiest to follow and understand.
    */
    void GetWallPrefab(int x, int y, ColourToPrefab colourMapping)
    {
        if (map.GetPixel(x + 1, y).Equals(colourMapping.colour)) 
        {
            if (map.GetPixel(x, y + 1).Equals(colourMapping.colour))
            {
                if (map.GetPixel(x - 1, y).Equals(colourMapping.colour))
                {
                    if (map.GetPixel(x, y - 1).Equals(colourMapping.colour))
                    {
                        // For some reason the cross shape was off centered, which is corrected by a y change of -0.645.
                        Instantiate(wall_X, new Vector3(x * 2, 1, (y * 2)-0.645f), Quaternion.Euler(new Vector3(-90, 0, 0)));
                    }
                    else
                    {
                        Instantiate(wall_T, new Vector3(x * 2, 1, y * 2), Quaternion.Euler(new Vector3(-90, 180, 0)));
                    }
                }
                else if (map.GetPixel(x, y - 1).Equals(colourMapping.colour))
                {
                    Instantiate(wall_T, new Vector3(x * 2, 1, y * 2), Quaternion.Euler(new Vector3(-90, -90, 0)));
                }
                else
                {
                    Instantiate(wall_L, new Vector3(x * 2, 1, y * 2), Quaternion.Euler(new Vector3(-90, -90, 0)));
                }
            }
            else if (map.GetPixel(x - 1, y).Equals(colourMapping.colour))
            {
                if (map.GetPixel(x, y - 1).Equals(colourMapping.colour))
                {
                    Instantiate(wall_T, new Vector3(x * 2, 1, y * 2), Quaternion.Euler(new Vector3(-90, 0, 0)));
                }
                else
                {
                    Instantiate(wall_I, new Vector3(x * 2, 1, y * 2), Quaternion.Euler(new Vector3(-90, 0, 0)));
                }
            }
            else if (map.GetPixel(x, y - 1).Equals(colourMapping.colour))
            {
                Instantiate(wall_L, new Vector3(x * 2, 1, y * 2), Quaternion.Euler(new Vector3(-90, 0, 0)));
            }
            else
            {
                Instantiate(wall_end, new Vector3(x * 2, 1, y * 2), Quaternion.Euler(new Vector3(-90, 0, 0)));
            }
        }

        else if (map.GetPixel(x - 1, y).Equals(colourMapping.colour))
        {
            if (map.GetPixel(x, y + 1).Equals(colourMapping.colour))
            {
                if (map.GetPixel(x, y - 1).Equals(colourMapping.colour))
                {
                    Instantiate(wall_T, new Vector3(x * 2, 1, y * 2), Quaternion.Euler(new Vector3(-90, 90, 0)));
                }
                else
                {
                    Instantiate(wall_L, new Vector3(x * 2, 1, y * 2), Quaternion.Euler(new Vector3(-90, 180, 0)));
                }
            }
            else if (map.GetPixel(x, y - 1).Equals(colourMapping.colour))
            {
                Instantiate(wall_L, new Vector3(x * 2, 1, y * 2), Quaternion.Euler(new Vector3(-90, 90, 0)));
            }
            else
            {
                Instantiate(wall_end, new Vector3(x * 2, 1, y * 2), Quaternion.Euler(new Vector3(-90, 180, 0)));
            }
        }

        else if (map.GetPixel(x, y + 1).Equals(colourMapping.colour))
        {
            if (map.GetPixel(x, y - 1).Equals(colourMapping.colour))
            {
                Instantiate(wall_I, new Vector3(x * 2, 1, y * 2), Quaternion.Euler(new Vector3(-90, 90, 0)));
            }
            else
            {
                Instantiate(wall_end, new Vector3(x * 2, 1, y * 2), Quaternion.Euler(new Vector3(-90, -90, 0)));
            }
        }

        else if (map.GetPixel(x, y - 1).Equals(colourMapping.colour))
        {
            Instantiate(wall_end, new Vector3(x * 2, 1, y * 2), Quaternion.Euler(new Vector3(-90, 90, 0)));
        }

        else
        {
            Instantiate(wall_o, new Vector3(x * 2, 1, y * 2), Quaternion.Euler(new Vector3(-90, 0, 0)));
        }
    }
}

