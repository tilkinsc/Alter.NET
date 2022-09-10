namespace Util.IO;

class IsaacRandom
{
	
	private const int GOLDEN_RATIO = -0x61C88647;
	private const int LOG_SIZE = 8;
	private const int SIZE = 1 << LOG_SIZE;
	private const int MASK = SIZE - 1 << 2;
	
	private int a = 0;
	private int b = 0;
	private int c = 0;
	private int count = 0;
	private int[] mem;
	private int[] rsl;
	
	public IsaacRandom()
	{
		mem = new int[SIZE];
		rsl = new int[SIZE];
		Init(false);
	}
	
	public IsaacRandom(int[] seed)
	{
		mem = new int[SIZE];
		rsl = new int[SIZE];
		for (int i=0; i<seed.Length; i++)
			rsl[i] = seed[i];
		Init(true);
	}
	
	private void Init(bool hasSeed)
	{
		int h = GOLDEN_RATIO;
		int g = GOLDEN_RATIO;
		int f = GOLDEN_RATIO;
		int e = GOLDEN_RATIO;
		int d = GOLDEN_RATIO;
		int c = GOLDEN_RATIO;
		int b = GOLDEN_RATIO;
		int a = GOLDEN_RATIO;
		int i = 0;
		while (i < 4) {
			a = a ^ (b << 11);
			d += a;
			b += c;
			b = b ^ (c >> 2);
			e += b;
			c += d;
			c = c ^ (d << 8);
			f += c;
			d += e;
			d = d ^ (e >> 16);
			g += d;
			e += f;
			e = e ^ (f << 10);
			h += e;
			f += g;
			f = f ^ (g >> 4);
			a += f;
			g += h;
			g = g ^ (h << 8);
			b += g;
			h += a;
			h = h ^ (a >> 9);
			c += h;
			a += b;
			++i;
		}
		i = 0;
		while (i < SIZE) { /* fill in mem[] with messy stuff */
			if (hasSeed) {
				a += rsl[i];
				b += rsl[i + 1];
				c += rsl[i + 2];
				d += rsl[i + 3];
				e += rsl[i + 4];
				f += rsl[i + 5];
				g += rsl[i + 6];
				h += rsl[i + 7];
			}
			a = a ^ (b << 11);
			d += a;
			b += c;
			b = b ^ (c >>2);
			e += b;
			c += d;
			c = c ^ (d << 8);
			f += c;
			d += e;
			d = d ^ (e >> 16);
			g += d;
			e += f;
			e = e ^ (f << 10);
			h += e;
			f += g;
			f = f ^ (g >> 4);
			a += f;
			g += h;
			g = g ^ (h << 8);
			b += g;
			h += a;
			h = h ^ (a >> 9);
			c += h;
			a += b;
			mem[i] = a;
			mem[i + 1] = b;
			mem[i + 2] = c;
			mem[i + 3] = d;
			mem[i + 4] = e;
			mem[i + 5] = f;
			mem[i + 6] = g;
			mem[i + 7] = h;
			i += 8;
		}
		if (hasSeed) { /* second pass makes all of seed affect all of mem */
			i = 0;
			while (i < SIZE) {
				a += mem[i];
				b += mem[i + 1];
				c += mem[i + 2];
				d += mem[i + 3];
				e += mem[i + 4];
				f += mem[i + 5];
				g += mem[i + 6];
				h += mem[i + 7];
				a = a ^ (b << 11);
				d += a;
				b += c;
				b = b ^ (c >> 2);
				e += b;
				c += d;
				c = c ^ (d << 8);
				f += c;
				d += e;
				d = d ^ (e >> 16);
				g += d;
				e += f;
				e = e ^ (f << 10);
				h += e;
				f += g;
				f = f ^ (g >> 4);
				a += f;
				g += h;
				g = g ^ (h << 8);
				b += g;
				h += a;
				h = h ^ (a >> 9);
				c += h;
				a += b;
				mem[i] = a;
				mem[i + 1] = b;
				mem[i + 2] = c;
				mem[i + 3] = d;
				mem[i + 4] = e;
				mem[i + 5] = f;
				mem[i + 6] = g;
				mem[i + 7] = h;
				i += 8;
			}
		}
		Isaac();
		count = SIZE;
	}
	
	private void Isaac()
	{
		int x = 0;
		int y = 0;
		b += ++c;
		int i = 0;
		int j = SIZE / 2;
		while (i < SIZE / 2) {
			x = mem[i];
			a = a ^ (a << 13);
			a += mem[j++];
			y = mem[x & MASK >> 2] + a + b;
			mem[i] = y;
			b = mem[y >> LOG_SIZE & MASK >> 2] + x;
			rsl[i++] = b;
			x = mem[i];
			a = a ^ (a >> 6);
			a += mem[j++];
			y = mem[x & MASK >> 2] + a + b;
			mem[i] = y;
			b = mem[y >> LOG_SIZE & MASK >> 2] + x;
			rsl[i++] = b;
			x = mem[i];
			a = a ^ (a << 2);
			a += mem[j++];
			y = mem[x & MASK >> 2] + a + b;
			mem[i] = y;
			b = mem[y >> LOG_SIZE & MASK >> 2] + x;
			rsl[i++] = b;
			x = mem[i];
			a = a ^ (a >> 16);
			a += mem[j++];
			y = mem[x & MASK >> 2] + a + b;
			mem[i] = y;
			b = mem[y >> LOG_SIZE & MASK >> 2] + x;
			rsl[i++] = b;
		}
		j = 0;
		while (j < SIZE / 2) {
			x = mem[i];
			a = a ^ (a << 13);
			a += mem[j++];
			y = mem[x & MASK >> 2] + a + b;
			mem[i] = y;
			b = mem[y >> LOG_SIZE & MASK >> 2] + x;
			rsl[i++] = b;
			x = mem[i];
			a = a ^ (a >> 6);
			a += mem[j++];
			y = mem[x & MASK >> 2] + a + b;
			mem[i] = y;
			b = mem[y >> LOG_SIZE & MASK >> 2] + x;
			rsl[i++] = b;
			x = mem[i];
			a = a ^ (a << 2);
			a += mem[j++];
			y = mem[x & MASK >> 2] + a + b;
			mem[i] = y;
			b = mem[y >> LOG_SIZE & MASK >> 2] + x;
			rsl[i++] = b;
			x = mem[i];
			a = a ^ (a >> 16);
			a += mem[j++];
			y = mem[x & MASK >> 2] + a + b;
			mem[i] = y;
			b = mem[y >> LOG_SIZE & MASK >> 2] + x;
			rsl[i++] = b;
		}
	}
	
	public int NextInt()
	{
		if (0 == count--) {
			Isaac();
			count = SIZE - 1;
		}
		return rsl[count];
	}
	
}
