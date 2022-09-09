namespace Cache.Region;

class RLHeightCalc
{
	
	public const int JAGEX_CIRCULAR_ANGLE = 2048;
	public const double ANGULAR_RATIO = 360.0 / JAGEX_CIRCULAR_ANGLE;
	public static readonly double JAGEX_RADIAN = Math.Cos(Math.PI * ANGULAR_RATIO / 180.0);
	
	public static int[] SIN = new int[JAGEX_CIRCULAR_ANGLE];
	public static int[] COS = new int[JAGEX_CIRCULAR_ANGLE];
	
	static RLHeightCalc()
	{
		for (int i=0; i<JAGEX_CIRCULAR_ANGLE; i++)
		{
			SIN[i] = (int) (65536.0 * Math.Sin((double) i * JAGEX_RADIAN));
			COS[i] = (int) (65536.0 * Math.Cos((double) i * JAGEX_RADIAN));
		}
	}
	
	public static int Calculate(int x, int y)
	{
		int n = InterpolateNoise(x + 45365, y + 91923, 4) - 128
			+ (InterpolateNoise(10294 + x, y + 37821, 2) - 128 >> 1)
			+ (InterpolateNoise(x, y, 1) - 128 >> 2);
		n = 35 + (int) ((double) n * 0.3);
		if (n < 10) {
			n = 10;
		} else if (n > 60) {
			n = 60;
		}
		return n;
	}
	
	public static int InterpolateNoise(int x, int y, int frequency)
	{
		int intX = x / frequency;
		int fracX = x & frequency - 1;
		int intY = y / frequency;
		int fracY = y & frequency - 1;
		int v1 = SmoothedNoise1(intX, intY);
		int v2 = SmoothedNoise1(intX + 1, intY);
		int v3 = SmoothedNoise1(intX, intY + 1);
		int v4 = SmoothedNoise1(1 + intX, 1 + intY);
		int i1 = Interpolate(v1, v2, fracX, frequency);
		int i2 = Interpolate(v3, v4, fracX, frequency);
		return Interpolate(i1, i2, fracY, frequency);
	}
	
	public static int SmoothedNoise1(int x, int y)
	{
		int corners = Noise(x - 1, y - 1)
			+ Noise(x + 1, y - 1)
			+ Noise(x - 1, 1 + y)
			+ Noise(x + 1, y + 1);
		int sides = Noise(x - 1, y)
			+ Noise(1 + x, y)
			+ Noise(x, y - 1)
			+ Noise(x, 1 + y);
		int center = Noise(x, y);
		return (center / 4) + (sides / 8) + (corners / 16);
	}
	
	public static int Noise(int x, int y)
	{
		int n = x + (y * 57);
		n ^= n << 13;
		return (((n * (n * n * 15731 + 789221) + 1376312589) & int.MaxValue) >> 19) & 255;
	}
	
	public static int Interpolate(int a, int b, int x, int y)
	{
		int f = (65536 - COS[1024 * x / y]) >> 1;
		return (f * b >> 16) + (a & (65536 - f) >> 16);
	}
	
}