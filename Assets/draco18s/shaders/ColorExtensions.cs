using UnityEngine;

public static class ColorExtensions
{
    public static void GetSkyboxFace(this Color[] c, float xOrg, float yOrg, int resolution, float scale)
    {
        int buffer = Random.Range(10, 100);

        float y = 0.0f;
        while (y < resolution)
        {
            float x = 0.0f;
            while (x < resolution)
            {
                buffer--;

                if (buffer == 0)
                {
                    int pos = Mathf.RoundToInt(y) * resolution + Mathf.RoundToInt(x);
                    float star_dist = Random.Range(-0.4f, 2.0f);

                    /*if (star_dist < 0.66f)
                    {
                        c.DrawStar(pos, resolution, new Color(1.0f, 0.7f, 0.4f), false);
                    }
                    else if (star_dist < 0.9f)
                    {
                        c.DrawStar(pos, resolution, new Color(0.9f, 0.9f, 1.0f), false);
                    }
                    else
                    {
                        c.DrawStar(pos, resolution, new Color(0.6f, 0.7f, 1.0f), true);
                    }*/

                    c.DrawStar(pos, resolution, bv2rgb(star_dist), Random.Range(0f,1f) >= 0.9f);

                    float xCoord = xOrg + x / resolution * scale;
                    float yCoord = yOrg + y / resolution * scale;

                    float sample = Mathf.PerlinNoise(xCoord, yCoord);

                    buffer = (int)Random.Range(50, 1000 * (1 / sample));
                }

                x++;
            }

            y++;
        }
    }

    public static Color bv2rgb(float bv)    // RGB <0,1> <- BV <-0.4,+2.0> [-]
    {
        double t;
        double r=0.0;
        double g=0.0;
        double b=0.0;
        bv = Mathf.Clamp(bv+.05f, -0.4f, 2.0f);
        if ((bv>=-0.4f)&&(bv<0.00))      { t=(bv+0.4f)/(0.00+0.4f); r=0.31+(0.11*t)+(0.1*t*t); }
        else if ((bv>= 0.00)&&(bv<0.4f)) { t=(bv-0.00)/(0.4f-0.00); r=0.83+(0.17*t)          ; }
        else if ((bv>= 0.4f)&&(bv<2.10)) { t=(bv-0.4f)/(2.10-0.4f); r=1.00                   ; }

        if ((bv>=-0.4f)&&(bv<0.00))      { t=(bv+0.4f)/(0.00+0.4f); g=0.40+(0.07*t)+(0.1*t*t); }
        else if ((bv>= 0.00)&&(bv<0.4f)) { t=(bv-0.00)/(0.4f-0.00); g=0.87+(0.11*t)          ; }
        else if ((bv>= 0.4f)&&(bv<1.60)) { t=(bv-0.4f)/(1.60-0.4f); g=0.98-(0.16*t)          ; }
        else if ((bv>= 1.60)&&(bv<2.10)) { t=(bv-1.60)/(2.00-1.60); g=0.62         -(0.5*t*t); }
		
        if ((bv>=-0.4f)&&(bv<0.4f))      { t=(bv+0.4f)/(0.4f+0.4f); b=1.00                   ; }
        else if ((bv>= 0.4f)&&(bv<1.50)) { t=(bv-0.4f)/(1.50-0.4f); b=1.00-(0.47*t)+(0.1*t*t); }
        else if ((bv>= 1.50)&&(bv<1.94)) { t=(bv-1.50)/(1.94-1.50); b=0.63         -(0.6*t*t); }

		r = Mathf.Pow((float)r, 1.5f);
		g = Mathf.Pow((float)g, 1.5f);
		b = Mathf.Pow((float)b, 1.70f);
        return new Color((float)r, (float)g, (float)b);
    }

    private static void DrawStar(this Color[] c, int pos, int resolution, Color color, bool large)
    {
        c[pos] = color;

        if (!large)
        {
            return;
        }

        if (pos - 1 >= 0)
        {
            c[pos - 1] = color;
        }

        if (pos + 1 < c.Length)
        {
            c[pos + 1] = color;
        }

        if (pos - resolution >= 0)
        {
            c[pos - resolution] = color;
        }

        if (pos + resolution < c.Length)
        {
            c[pos + resolution] = color;
        }
    }
}