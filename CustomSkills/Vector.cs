using System;
using System.Globalization;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace CustomSkill
{
	[TypeConverter(typeof(VectorConverter))]
	[JsonConverter(typeof(VectorConverterJ))]
	public struct Vector
	{
		public static Vector[] Array;

		public float X;

		public float Y;

		private static Vector _zero = default(Vector);

		private static Vector _one = new Vector(1f, 1f);

		private static Vector _unitX = new Vector(1f, 0f);

		private static Vector _unitY = new Vector(0f, 1f);

		public static Vector Zero => _zero;

		public static Vector One => _one;

		public static Vector UnitX => _unitX;

		public static Vector UnitY => _unitY;

		public Vector(double x, double y) : this((float)x, (float)y)
		{

		}

		public Vector(float x, float y)
		{
			X = x;
			Y = y;
		}

		public Vector(float value)
		{
			Y = value;
			X = value;
		}

		public static Vector FromPolar(double angle, float len)
		{
			return new Vector
			{
				X = (float)Math.Cos(angle) * len,
				Y = (float)Math.Sin(angle) * len
			};
		}

		public float PolarRadius
		{
			get
			{
				return Length();
			}
			set
			{
				var angle = Angle;
				(X, Y) = ((float, float))(Math.Cos(angle) * value, Math.Sin(angle) * value);
			}
		}

		public double Angle
		{
			get
			{
				return Math.Atan2(Y, X);
			}
			set
			{
				var len = PolarRadius;
				(X, Y) = ((float, float))(Math.Cos(value) * len, Math.Sin(value) * len);
			}
		}

		public override string ToString()
		{
			return $"({X:0.00},{(Y < 0 ? "" : " ")}{Y:0.00})";
		}

		public string ToString(string format)
		{
			return X.ToString(format) + ", " + Y.ToString(format);
		}

		public Vector Deflect(double angle) => Vector.FromPolar(Angle + angle, PolarRadius);

		public Vector ToLenOf(float len)
		{
			Vector result = this;
			result.PolarRadius = len;
			return result;
		}
		public Vector ToAngleOf(double angle)
		{
			Vector result = this;
			result.Angle = angle;
			return result;
		}
		public Vector ToVertical()
		{
			return (-Y, X);
		}
		public Vector ToVertical(float len)
		{
			return FromPolar(Angle + Math.PI / 2, len);
		}

		public Vector AddVertical(float len)
		{
			return this + ToVertical(len);
		}

		public bool Equals(Vector other)
		{
			return X == other.X && Y == other.Y;
		}

		public override bool Equals(object obj)
		{
			bool result = false;
			if (obj is Vector)
			{
				result = Equals((Vector)obj);
			}
			return result;
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() + Y.GetHashCode();
		}

		public float Length()
		{
			float num = X * X + Y * Y;
			return (float)Math.Sqrt(num);
		}

		public float LengthSquared()
		{
			return X * X + Y * Y;
		}

		public static float Distance(Vector value1, Vector value2)
		{
			float num = value1.X - value2.X;
			float num2 = value1.Y - value2.Y;
			float num3 = num * num + num2 * num2;
			return (float)Math.Sqrt(num3);
		}

		public static void Distance(ref Vector value1, ref Vector value2, out float result)
		{
			float num = value1.X - value2.X;
			float num2 = value1.Y - value2.Y;
			float num3 = num * num + num2 * num2;
			result = (float)Math.Sqrt(num3);
		}

		public static float DistanceSquared(Vector value1, Vector value2)
		{
			float num = value1.X - value2.X;
			float num2 = value1.Y - value2.Y;
			return num * num + num2 * num2;
		}

		public static void DistanceSquared(ref Vector value1, ref Vector value2, out float result)
		{
			float num = value1.X - value2.X;
			float num2 = value1.Y - value2.Y;
			result = num * num + num2 * num2;
		}

		public static float Dot(Vector value1, Vector value2)
		{
			return value1.X * value2.X + value1.Y * value2.Y;
		}

		public static void Dot(ref Vector value1, ref Vector value2, out float result)
		{
			result = value1.X * value2.X + value1.Y * value2.Y;
		}

		public void Normalize()
		{
			float num = X * X + Y * Y;
			float num2 = 1f / (float)Math.Sqrt(num);
			X *= num2;
			Y *= num2;
		}

		public static Vector Normalize(Vector value)
		{
			float num = value.X * value.X + value.Y * value.Y;
			float num2 = 1f / (float)Math.Sqrt(num);
			Vector result = default(Vector);
			result.X = value.X * num2;
			result.Y = value.Y * num2;
			return result;
		}

		public static void Normalize(ref Vector value, out Vector result)
		{
			float num = value.X * value.X + value.Y * value.Y;
			float num2 = 1f / (float)Math.Sqrt(num);
			result.X = value.X * num2;
			result.Y = value.Y * num2;
		}

		public static Vector Reflect(Vector vector, Vector normal)
		{
			float num = vector.X * normal.X + vector.Y * normal.Y;
			Vector result = default(Vector);
			result.X = vector.X - 2f * num * normal.X;
			result.Y = vector.Y - 2f * num * normal.Y;
			return result;
		}

		public static void Reflect(ref Vector vector, ref Vector normal, out Vector result)
		{
			float num = vector.X * normal.X + vector.Y * normal.Y;
			result.X = vector.X - 2f * num * normal.X;
			result.Y = vector.Y - 2f * num * normal.Y;
		}

		public static Vector Min(Vector value1, Vector value2)
		{
			Vector result = default(Vector);
			result.X = ((value1.X < value2.X) ? value1.X : value2.X);
			result.Y = ((value1.Y < value2.Y) ? value1.Y : value2.Y);
			return result;
		}

		public static void Min(ref Vector value1, ref Vector value2, out Vector result)
		{
			result.X = ((value1.X < value2.X) ? value1.X : value2.X);
			result.Y = ((value1.Y < value2.Y) ? value1.Y : value2.Y);
		}

		public static Vector Max(Vector value1, Vector value2)
		{
			Vector result = default(Vector);
			result.X = ((value1.X > value2.X) ? value1.X : value2.X);
			result.Y = ((value1.Y > value2.Y) ? value1.Y : value2.Y);
			return result;
		}

		public static void Max(ref Vector value1, ref Vector value2, out Vector result)
		{
			result.X = ((value1.X > value2.X) ? value1.X : value2.X);
			result.Y = ((value1.Y > value2.Y) ? value1.Y : value2.Y);
		}

		public static Vector Clamp(Vector value1, Vector min, Vector max)
		{
			float x = value1.X;
			x = ((x > max.X) ? max.X : x);
			x = ((x < min.X) ? min.X : x);
			float y = value1.Y;
			y = ((y > max.Y) ? max.Y : y);
			y = ((y < min.Y) ? min.Y : y);
			Vector result = default(Vector);
			result.X = x;
			result.Y = y;
			return result;
		}

		public static void Clamp(ref Vector value1, ref Vector min, ref Vector max, out Vector result)
		{
			float x = value1.X;
			x = ((x > max.X) ? max.X : x);
			x = ((x < min.X) ? min.X : x);
			float y = value1.Y;
			y = ((y > max.Y) ? max.Y : y);
			y = ((y < min.Y) ? min.Y : y);
			result.X = x;
			result.Y = y;
		}

		public static Vector Lerp(Vector value1, Vector value2, float amount)
		{
			Vector result = default(Vector);
			result.X = value1.X + (value2.X - value1.X) * amount;
			result.Y = value1.Y + (value2.Y - value1.Y) * amount;
			return result;
		}

		public static void Lerp(ref Vector value1, ref Vector value2, float amount, out Vector result)
		{
			result.X = value1.X + (value2.X - value1.X) * amount;
			result.Y = value1.Y + (value2.Y - value1.Y) * amount;
		}

		public static Vector Barycentric(Vector value1, Vector value2, Vector value3, float amount1, float amount2)
		{
			Vector result = default(Vector);
			result.X = value1.X + amount1 * (value2.X - value1.X) + amount2 * (value3.X - value1.X);
			result.Y = value1.Y + amount1 * (value2.Y - value1.Y) + amount2 * (value3.Y - value1.Y);
			return result;
		}

		public static void Barycentric(ref Vector value1, ref Vector value2, ref Vector value3, float amount1, float amount2, out Vector result)
		{
			result.X = value1.X + amount1 * (value2.X - value1.X) + amount2 * (value3.X - value1.X);
			result.Y = value1.Y + amount1 * (value2.Y - value1.Y) + amount2 * (value3.Y - value1.Y);
		}

		public static Vector SmoothStep(Vector value1, Vector value2, float amount)
		{
			amount = ((amount > 1f) ? 1f : ((amount < 0f) ? 0f : amount));
			amount = amount * amount * (3f - 2f * amount);
			Vector result = default(Vector);
			result.X = value1.X + (value2.X - value1.X) * amount;
			result.Y = value1.Y + (value2.Y - value1.Y) * amount;
			return result;
		}

		public static void SmoothStep(ref Vector value1, ref Vector value2, float amount, out Vector result)
		{
			amount = ((amount > 1f) ? 1f : ((amount < 0f) ? 0f : amount));
			amount = amount * amount * (3f - 2f * amount);
			result.X = value1.X + (value2.X - value1.X) * amount;
			result.Y = value1.Y + (value2.Y - value1.Y) * amount;
		}

		public static Vector CatmullRom(Vector value1, Vector value2, Vector value3, Vector value4, float amount)
		{
			float num = amount * amount;
			float num2 = amount * num;
			Vector result = default(Vector);
			result.X = 0.5f * (2f * value2.X + (0f - value1.X + value3.X) * amount + (2f * value1.X - 5f * value2.X + 4f * value3.X - value4.X) * num + (0f - value1.X + 3f * value2.X - 3f * value3.X + value4.X) * num2);
			result.Y = 0.5f * (2f * value2.Y + (0f - value1.Y + value3.Y) * amount + (2f * value1.Y - 5f * value2.Y + 4f * value3.Y - value4.Y) * num + (0f - value1.Y + 3f * value2.Y - 3f * value3.Y + value4.Y) * num2);
			return result;
		}

		public static void CatmullRom(ref Vector value1, ref Vector value2, ref Vector value3, ref Vector value4, float amount, out Vector result)
		{
			float num = amount * amount;
			float num2 = amount * num;
			result.X = 0.5f * (2f * value2.X + (0f - value1.X + value3.X) * amount + (2f * value1.X - 5f * value2.X + 4f * value3.X - value4.X) * num + (0f - value1.X + 3f * value2.X - 3f * value3.X + value4.X) * num2);
			result.Y = 0.5f * (2f * value2.Y + (0f - value1.Y + value3.Y) * amount + (2f * value1.Y - 5f * value2.Y + 4f * value3.Y - value4.Y) * num + (0f - value1.Y + 3f * value2.Y - 3f * value3.Y + value4.Y) * num2);
		}

		public static Vector Hermite(Vector value1, Vector tangent1, Vector value2, Vector tangent2, float amount)
		{
			float num = amount * amount;
			float num2 = amount * num;
			float num3 = 2f * num2 - 3f * num + 1f;
			float num4 = -2f * num2 + 3f * num;
			float num5 = num2 - 2f * num + amount;
			float num6 = num2 - num;
			Vector result = default(Vector);
			result.X = value1.X * num3 + value2.X * num4 + tangent1.X * num5 + tangent2.X * num6;
			result.Y = value1.Y * num3 + value2.Y * num4 + tangent1.Y * num5 + tangent2.Y * num6;
			return result;
		}

		public static void Hermite(ref Vector value1, ref Vector tangent1, ref Vector value2, ref Vector tangent2, float amount, out Vector result)
		{
			float num = amount * amount;
			float num2 = amount * num;
			float num3 = 2f * num2 - 3f * num + 1f;
			float num4 = -2f * num2 + 3f * num;
			float num5 = num2 - 2f * num + amount;
			float num6 = num2 - num;
			result.X = value1.X * num3 + value2.X * num4 + tangent1.X * num5 + tangent2.X * num6;
			result.Y = value1.Y * num3 + value2.Y * num4 + tangent1.Y * num5 + tangent2.Y * num6;
		}

		public static Vector Negate(Vector value)
		{
			Vector result = default(Vector);
			result.X = 0f - value.X;
			result.Y = 0f - value.Y;
			return result;
		}

		public static void Negate(ref Vector value, out Vector result)
		{
			result.X = 0f - value.X;
			result.Y = 0f - value.Y;
		}

		public static Vector Add(Vector value1, Vector value2)
		{
			Vector result = default(Vector);
			result.X = value1.X + value2.X;
			result.Y = value1.Y + value2.Y;
			return result;
		}

		public static void Add(ref Vector value1, ref Vector value2, out Vector result)
		{
			result.X = value1.X + value2.X;
			result.Y = value1.Y + value2.Y;
		}

		public static Vector Subtract(Vector value1, Vector value2)
		{
			Vector result = default(Vector);
			result.X = value1.X - value2.X;
			result.Y = value1.Y - value2.Y;
			return result;
		}

		public static void Subtract(ref Vector value1, ref Vector value2, out Vector result)
		{
			result.X = value1.X - value2.X;
			result.Y = value1.Y - value2.Y;
		}

		public static Vector Multiply(Vector value1, Vector value2)
		{
			Vector result = default(Vector);
			result.X = value1.X * value2.X;
			result.Y = value1.Y * value2.Y;
			return result;
		}

		public static void Multiply(ref Vector value1, ref Vector value2, out Vector result)
		{
			result.X = value1.X * value2.X;
			result.Y = value1.Y * value2.Y;
		}

		public static Vector Multiply(Vector value1, float scaleFactor)
		{
			Vector result = default(Vector);
			result.X = value1.X * scaleFactor;
			result.Y = value1.Y * scaleFactor;
			return result;
		}

		public static void Multiply(ref Vector value1, float scaleFactor, out Vector result)
		{
			result.X = value1.X * scaleFactor;
			result.Y = value1.Y * scaleFactor;
		}

		public static Vector Divide(Vector value1, Vector value2)
		{
			Vector result = default(Vector);
			result.X = value1.X / value2.X;
			result.Y = value1.Y / value2.Y;
			return result;
		}

		public static void Divide(ref Vector value1, ref Vector value2, out Vector result)
		{
			result.X = value1.X / value2.X;
			result.Y = value1.Y / value2.Y;
		}

		public static Vector Divide(Vector value1, float divider)
		{
			float num = 1f / divider;
			Vector result = default(Vector);
			result.X = value1.X * num;
			result.Y = value1.Y * num;
			return result;
		}

		public static void Divide(ref Vector value1, float divider, out Vector result)
		{
			float num = 1f / divider;
			result.X = value1.X * num;
			result.Y = value1.Y * num;
		}

		public static Vector operator -(Vector value)
		{
			Vector result = default(Vector);
			result.X = 0f - value.X;
			result.Y = 0f - value.Y;
			return result;
		}

		public static bool operator ==(Vector value1, Vector value2)
		{
			return value1.X == value2.X && value1.Y == value2.Y;
		}

		public static bool operator !=(Vector value1, Vector value2)
		{
			return value1.X != value2.X || value1.Y != value2.Y;
		}

		public static Vector operator +(Vector value1, Vector value2)
		{
			Vector result = default(Vector);
			result.X = value1.X + value2.X;
			result.Y = value1.Y + value2.Y;
			return result;
		}

		public static Vector operator -(Vector value1, Vector value2)
		{
			Vector result = default(Vector);
			result.X = value1.X - value2.X;
			result.Y = value1.Y - value2.Y;
			return result;
		}

		public static Vector operator *(Vector value1, Vector value2)
		{
			Vector result = default(Vector);
			result.X = value1.X * value2.X;
			result.Y = value1.Y * value2.Y;
			return result;
		}

		public static Vector operator *(Vector value, float scaleFactor)
		{
			Vector result = default(Vector);
			result.X = value.X * scaleFactor;
			result.Y = value.Y * scaleFactor;
			return result;
		}

		public static Vector operator *(float scaleFactor, Vector value)
		{
			Vector result = default;
			result.X = value.X * scaleFactor;
			result.Y = value.Y * scaleFactor;
			return result;
		}

		public static Vector operator /(Vector value1, Vector value2)
		{
			Vector result = default(Vector);
			result.X = value1.X / value2.X;
			result.Y = value1.Y / value2.Y;
			return result;
		}

		public static Vector operator /(Vector value1, float divider)
		{
			float num = 1f / divider;
			Vector result = default(Vector);
			result.X = value1.X * num;
			result.Y = value1.Y * num;
			return result;
		}
		public void Deconstruct(out float x, out float y)
		{
			(x, y) = (X, Y);
		}

		public static implicit operator Vector((double X, double Y) value) => new Vector((float)value.X, (float)value.Y);
		public static implicit operator Vector((float X, float Y) value) => new Vector(value.X, value.Y);

		public static implicit operator Vector2(Vector value) => new Vector2(value.X, value.Y);
		public static implicit operator Vector(Vector2 value) => new Vector(value.X, value.Y);

	}
	internal class VectorConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string)) return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string)) return true;
			return base.CanConvertTo(context, destinationType);
		}
		public new static Vector ConvertFrom(object value)
		{
			var match = Regex.Match((string)value, @"\(?(.*),\b?([^)]*)\)?");
			return (double.Parse(match.Groups[1].Value), double.Parse(match.Groups[2].Value));
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return ConvertFrom(value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			return value?.ToString();
		}
	}
	internal class VectorConverterJ : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Vector);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return VectorConverter.ConvertFrom(reader.Value);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteValue(value.ToString());
		}
	}
}
