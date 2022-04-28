using UnityEngine;
using System;
using System.Text;
//using Koopakiller.Numerics;

namespace Assets.draco18s.util {
	public static class MathHelper {
		public static double PIOver180D = System.Math.PI / 180.0;
		public static double PID = 3.14159265358979323846264338327950288419716939937510;

		public static int getLayerForMask(int maskVal) {
			int j = 1;
			while((maskVal & 1) == 0) {
				maskVal /= 2;
				j++;
			}
			return j;
		}

		public static float EaseLinear(float time, float from, float to, float duration) {
			time /= duration;
			float by = to - from;
			return by * time + from;
		}

		public static float EaseInQuadratic(float time, float from, float to, float duration) {
			time /= duration;
			float by = to - from;
			return by * time * time + from;
		}

		public static float EaseInCubic(float time, float from, float to, float duration) {
			time /= duration;
			float by = to - from;
			return by * time * time * time + from;
		}

		public static float EaseInQuartic(float time, float from, float to, float duration) {
			time /= duration;
			float by = to - from;
			return by * time * time * time * time + from;
		}

		public static float EaseOutQuadratic(float time, float from, float to, int duration) {
			time /= duration;
			float by = to - from;
			return -by * time * (time - 2) + from;
		}

		public static double DegreesToRadiansD(double Degrees) {
			return Degrees * PIOver180D;
		}

		public static float RadiansToDegrees(float Radians) {
			return Radians / ((float)Mathf.PI / 180f);
		}

		public static string ToRoman(int value) {
			if(value < 0)
				throw new ArgumentOutOfRangeException("Please use a positive integer greater than zero.");

			StringBuilder sb = new StringBuilder();
			int remain = value;
			while(remain > 0) {
				if(remain >= 1000) { sb.Append("M"); remain -= 1000; }
				else if(remain >= 900) { sb.Append("CM"); remain -= 900; }
				else if(remain >= 500) { sb.Append("D"); remain -= 500; }
				else if(remain >= 400) { sb.Append("CD"); remain -= 400; }
				else if(remain >= 100) { sb.Append("C"); remain -= 100; }
				else if(remain >= 90) { sb.Append("XC"); remain -= 90; }
				else if(remain >= 50) { sb.Append("L"); remain -= 50; }
				else if(remain >= 40) { sb.Append("XL"); remain -= 40; }
				else if(remain >= 10) { sb.Append("X"); remain -= 10; }
				else if(remain >= 9) { sb.Append("IX"); remain -= 9; }
				else if(remain >= 5) { sb.Append("V"); remain -= 5; }
				else if(remain >= 4) { sb.Append("IV"); remain -= 4; }
				else if(remain >= 1) { sb.Append("I"); remain -= 1; }
				else throw new Exception("Unexpected error."); // <<-- shouldn't be possble to get here, but it ensures that we will never have an infinite loop (in case the computer is on crack that day).
			}

			return sb.ToString();
		}

		public static double SetAngleAndEnsureWithinRange(double newAngle, double angleCeiling) {
			double acutalAngle;

			acutalAngle = newAngle;
			if(acutalAngle < 0f)
				acutalAngle += angleCeiling;
			if(acutalAngle >= angleCeiling)
				acutalAngle -= angleCeiling;
			return acutalAngle;
		}

		public static Vector3 snap(Vector3 pos, int v) {
			float x = pos.x;
			float y = pos.y;
			float z = pos.z;
			x = snap(x, v);
			y = snap(y, v);
			z = snap(z, v);
			return new Vector3(x, y, z);
		}

		public static int snap(int pos, int v) {
			float x = pos;
			return Mathf.FloorToInt(x / v) * v;
		}

		public static float snap(float pos, float v) {
			float x = pos;
			return Mathf.FloorToInt(x / v) * v;
		}

		public static float SafeAngleAddDegrees(float Angle, float AmountToAdd) {
			Angle += AmountToAdd;
			while(Angle < 0f)
				Angle += 360f;
			while(Angle >= 360f)
				Angle -= 360f;
			return Angle;
		}

		public static double SafeAngleAddDegrees(double Angle, double AmountToAdd) {
			Angle += AmountToAdd;
			while(Angle < 0f)
				Angle += 360;
			while(Angle >= 360)
				Angle -= 360;
			return Angle;
		}

		public static float SafeAngleAddRadians(float Angle, float AmountToAdd) {
			Angle += AmountToAdd;
			while(Angle < 0f)
				Angle += Mathf.PI;
			while(Angle >= Mathf.PI)
				Angle -= Mathf.PI;
			return Angle;
		}

		public static double SafeAngleAddRadians(double Angle, double AmountToAdd) {
			Angle += AmountToAdd;
			while(Angle < 0)
				Angle += 2 * System.Math.PI;
			while(Angle >= 2 * System.Math.PI)
				Angle -= 2 * System.Math.PI;
			return Angle;
		}

		public static double AngleBetweenPointsDegreesDouble(Vector2 P1, Vector2 P2) {
			return AngleBetweenPointsRadiansDouble(P1.x, P1.y, P2.x, P2.y) * (180 / PID);
		}

		public static double AngleBetweenPointsDegreesDouble(float P1X, float P1Y, float P2X, float P2Y) {
			return AngleBetweenPointsRadiansDouble(P1X, P1Y, P2X, P2Y) * (180 / PID);
		}

		public static double AngleBetweenPointsRadiansDouble(Vector2 P1, Vector2 P2) {
			return Math.Atan2((P2.y - P1.y), (P2.x - P1.x));
		}

		public static double AngleBetweenPointsRadiansDouble(float P1X, float P1Y, float P2X, float P2Y) {
			P2X -= P1X;
			P2Y -= P1Y;
			P1X = 0;
			P1Y = 0;
			return Math.Atan2((P2Y - P1Y), (P2X - P1X));
		}

		public static int ApproxDistanceBetweenPointsFast(Vector2 P1, Vector2 P2, int ShortcutsBeyond) {
			return ApproxDistanceBetweenPointsFast(
				Mathf.RoundToInt(P1.x),
				Mathf.RoundToInt(P1.y),
				Mathf.RoundToInt(P2.x),
				Mathf.RoundToInt(P2.y),
				ShortcutsBeyond);
		}

		public static int ApproxDistanceBetweenPointsFast(int P1x, int P1y, int P2x, int P2y, Int32 ShortcutsBeyond) {
			Int32 dx = P1x - P2x;
			Int32 dy = P1y - P2y;

			if(dx < 0) dx = -dx;
			if(dy < 0) dy = -dy;

			if(ShortcutsBeyond > 0) {
				if(dx > ShortcutsBeyond || dy > ShortcutsBeyond) {
					if(dx > dy)
						return dx;
					else
						return dy;
				}
			}

			Int64 min, max, approx;

			if(dx < dy) {
				min = dx;
				max = dy;
			}
			else {
				min = dy;
				max = dx;
			}

			approx = (max * 1007) + (min * 441);
			if(max < (min << 4))
				approx -= (max * 40);

			// add 512 for proper rounding
			Int32 val = (Int32)((approx + 512) >> 10);
			if(val < 0)
				return -val;
			return val;
		}

		/*public static BigInteger Min(BigInteger amt, BigInteger maxSell) {
			return amt > maxSell ? maxSell : amt;
		}

		public static BigInteger Max(BigInteger amt, BigInteger maxSell) {
			return amt < maxSell ? maxSell : amt;
		}*/

		public static float DistanceBetweenPoints(Vector2 P1, Vector2 P2) {
			float xd = P2.x - P1.x;
			float yd = P2.y - P1.y;
			return (Mathf.Abs(Mathf.Sqrt(Mathf.Abs(
				(((long)xd * (long)xd) + ((long)yd * (long)yd))))));
		}

		public static float VeryBasicWrongRange(Vector2 P1, Vector2 P2) {
			float xDiff = P1.x - P2.x;
			if(xDiff < 0)
				xDiff = -xDiff;

			float yDiff = P1.y - P2.y;
			if(yDiff < 0)
				yDiff = -yDiff;

			if(xDiff > yDiff)
				return xDiff;
			else
				return yDiff;
		}

		public static double GetShortestAngleDeltaDegrees(double FromAngle, double ToAngle) {
			double deltaUp;
			double deltaDown;

			if(FromAngle <= ToAngle) {
				deltaUp = ToAngle - FromAngle;
				deltaDown = FromAngle + (360.0 - ToAngle);
			}
			else {
				deltaUp = (360.0 - FromAngle) + ToAngle;
				deltaDown = FromAngle - ToAngle;
			}

			if(Math.Abs(deltaUp) <= Math.Abs(deltaDown))
				return deltaUp;
			else
				return -deltaDown;
		}

		public static void GetDistanceAndAngle(Vector2 point, out double distance, out double angle) {
			distance = point.magnitude;
			angle = MathHelper.SafeAngleAddRadians(0, MathHelper.AngleBetweenPointsRadiansDouble(Vector2.zero, point));
		}

		public static Vector2 GetPointFromCircleCenterV(Vector2 Center, double DistanceFromCenter, double DegreesAngleFromCenter) {
			double baseX = DistanceFromCenter * Math.Cos(MathHelper.DegreesToRadiansD(DegreesAngleFromCenter));
			double baseY = DistanceFromCenter * Math.Sin(MathHelper.DegreesToRadiansD(DegreesAngleFromCenter));
			return new Vector2((float)(Center.x + baseX), (float)(Center.y + baseY));
		}

		public static void shuffleArray<T>(ref T[] array, System.Random random) {
			for(int i = array.Length - 1; i > 0; i--) {
				int index = random.Next(i + 1);
				T a = array[index];
				array[index] = array[i];
				array[i] = a;
			}
		}

		public static Vector2 VectorForAngle(float currentAngleRadians) {
			float sin = Mathf.Sin(currentAngleRadians);
			float cos = Mathf.Cos(currentAngleRadians);
			return new Vector2(sin, cos);
		}

		//
		// Public stuff
		//

		// Gamma function

		/**
		* log Gamma function: ln(gamma(alpha)) for alpha>0, accurate to 10 decimal places
		*
		* @param alpha argument
		* @return the log of the gamma function of the given alpha
		*/
		public static double lnGamma(double alpha) {
			// Pike MC & Hill ID (1966) Algorithm 291: Logarithm of the gamma function.
			// Communications of the Association for Computing Machinery, 9:684

			double x = alpha, f = 0.0, z;

			if (x < 7) {
				f = 1;
				z = x - 1;
				while (++z < 7) {
					f *= z;
				}
				x = z;
				f = -Math.Log(f);
			}
			z = 1 / (x * x);

			return f + (x - 0.5) * Math.Log(x) - x + 0.918938533204673 +
					(((-0.000595238095238 * z + 0.000793650793651) *
					z - 0.002777777777778) * z + 0.083333333333333) / x;
		}

		/**
		* Incomplete Gamma function Q(a,x)
		* (a cleanroom implementation of Numerical Recipes gammq(a,x);
		* in Mathematica this function is called GammaRegularized)
		*
		* @param a parameter
		* @param x argument
		* @return function value
		*/
		public static double incompleteGammaQ(double a, double x) {
			return 1.0 - incompleteGamma(x, a, lnGamma(a));
		}

		/**
		* Incomplete Gamma function P(a,x) = 1-Q(a,x)
		* (a cleanroom implementation of Numerical Recipes gammp(a,x);
		* in Mathematica this function is 1-GammaRegularized)
		*
		* @param a parameter
		* @param x argument
		* @return function value
		*/
		public static double incompleteGammaP(double a, double x) {
			return incompleteGamma(x, a, lnGamma(a));
		}

		/**
		* Incomplete Gamma function P(a,x) = 1-Q(a,x)
		* (a cleanroom implementation of Numerical Recipes gammp(a,x);
		* in Mathematica this function is 1-GammaRegularized)
		*
		* @param a        parameter
		* @param x        argument
		* @param lnGammaA precomputed lnGamma(a)
		* @return function value
		*/
		public static double incompleteGammaP(double a, double x, double lnGammaA) {
			return incompleteGamma(x, a, lnGammaA);
		}


		/**
		* Returns the incomplete gamma ratio I(x,alpha) where x is the upper
		* limit of the integration and alpha is the shape parameter.
		*
		* @param x              upper limit of integration
		* @param alpha          shape parameter
		* @param ln_gamma_alpha the log gamma function for alpha
		* @return the incomplete gamma ratio
		*/
		private static double incompleteGamma(double x, double alpha, double ln_gamma_alpha) {
			// (1) series expansion     if (alpha>x || x<=1)
			// (2) continued fraction   otherwise
			// RATNEST FORTRAN by
			// Bhattacharjee GP (1970) The incomplete gamma integral.  Applied Statistics,
			// 19: 285-287 (AS32)

			double accurate = 1e-8, overflow = 1e30;
			double factor, gin, rn, a, b, an, dif, term;
			double pn0, pn1, pn2, pn3, pn4, pn5;

			if (x == 0.0) {
				return 0.0;
			}
			if (x < 0.0 || alpha <= 0.0) {
				throw new Exception("Arguments out of bounds");
			}

			factor = Math.Exp(alpha * Math.Log(x) - x - ln_gamma_alpha);

			if (x > 1 && x >= alpha) {
				// continued fraction
				a = 1 - alpha;
				b = a + x + 1;
				term = 0;
				pn0 = 1;
				pn1 = x;
				pn2 = x + 1;
				pn3 = x * b;
				gin = pn2 / pn3;

				do {
					a++;
					b += 2;
					term++;
					an = a * term;
					pn4 = b * pn2 - an * pn0;
					pn5 = b * pn3 - an * pn1;

					if (pn5 != 0) {
						rn = pn4 / pn5;
						dif = Math.Abs(gin - rn);
						if (dif <= accurate) {
							if (dif <= accurate * rn) {
								break;
							}
						}

						gin = rn;
					}
					pn0 = pn2;
					pn1 = pn3;
					pn2 = pn4;
					pn3 = pn5;
					if (Math.Abs(pn4) >= overflow) {
						pn0 /= overflow;
						pn1 /= overflow;
						pn2 /= overflow;
						pn3 /= overflow;
					}
				} while (true);
				gin = 1 - factor * gin;
			} else {
				// series expansion
				gin = 1;
				term = 1;
				rn = alpha;
				do {
					rn++;
					term *= x / rn;
					gin += term;
				}
				while (term > accurate);
				gin *= factor / alpha;
			}
			return gin;
		}

		private static float ComplementaryErrorFunc(float x) {
			return 1 - (0.56418f*(float)incompleteGammaP(0.5,x*x));
		}

		/*public static float Wald(float x) {
			float f = Math.abs(standardize(x));
			float m = (float)Math.sqrt(1/x);
			//taken from Wolfram Alpha "graph cumulative wald distribution mean 1 scale 0.2"
			return 0.5f*ComplementaryErrorFunc(0.316228f*(1-x)*m) + 0.745912f*ComplementaryErrorFunc(0.316228f*(1+x)*m);
		}*/
	}
}