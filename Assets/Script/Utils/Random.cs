using UnityEngine;
using System.Collections;

namespace Utils
{
	public class Random 
	{
		static uint gen1 = 1664525;
		static uint gen2 = 1013904223;
		static uint random_seed = 0;
		static uint seed_max = 4294967295;

		public static void Seed(uint seed)
		{
			random_seed = seed;
		}

		static uint R()
		{
			uint newseed = gen1 * random_seed + gen2;

			newseed = newseed % seed_max;

			random_seed = newseed;

			return random_seed;	
		}

		public static uint Rand(uint min, uint max)
		{
			uint r = Random.R();

			return r % (max - min + 1) + min;
		}

		public static uint Rand(uint max)
		{
			uint r = Random.R();

			return r % max + 1;
		}

		public static float Rand()
		{
			uint r = Random.R();

			return r * 1.0f / seed_max;
		}
	}	
}

