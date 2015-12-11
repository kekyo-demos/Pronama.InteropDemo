////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Pronama.InteropDemo - How to use Win32 API in .NET 
// Copyright (c) Kouji Matsui, All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
// * Redistributions of source code must retain the above copyright notice,
//   this list of conditions and the following disclaimer.
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
// IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
// INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pronama.InteropDemo.Internals
{
	public static class Utilities
	{
		/// <summary>
		/// PointをVectorに変換します。
		/// </summary>
		/// <param name="point">Point</param>
		/// <returns>Vector</returns>
		/// <remarks>ベクトル演算に使用します。</remarks>
		public static Vector ToVector(this Point point)
		{
			return new Vector(point.X, point.Y);
		}

		/// <summary>
		/// VectorをPointに変換します。
		/// </summary>
		/// <param name="vector">Vector</param>
		/// <returns>Point</returns>
		/// <remarks>ベクトル演算に使用します。</remarks>
		public static Point ToPoint(this Vector vector)
		{
			return new Point(vector.X, vector.Y);
		}

		/// <summary>
		/// 二つのベクトルの外積を求めます。
		/// </summary>
		/// <param name="a">ベクトル</param>
		/// <param name="b">ベクトル</param>
		/// <returns>結果</returns>
		public static double CrossProduct(Vector a, Vector b)
		{
			return a.X * b.Y - a.Y * b.X;
		}

		/// <summary>
		/// 線分が交差しているかどうかを判定します。
		/// </summary>
		/// <param name="a1">線分aの始点</param>
		/// <param name="a2">線分aの終点</param>
		/// <param name="b1">線分bの始点</param>
		/// <param name="b2">線分bの終点</param>
		/// <returns>交差していればtrue</returns>
		public static bool IsIntersect(Vector a1, Vector a2, Vector b1, Vector b2)
		{
			return
				(CrossProduct(a2 - a1, b1 - a1) * CrossProduct(a2 - a1, b2 - a1) < Double.Epsilon) &&
				(CrossProduct(b2 - b1, a1 - b1) * CrossProduct(b2 - b1, a2 - b1) < Double.Epsilon);
		}

		/// <summary>
		/// 線分の交点を求めます
		/// </summary>
		/// <param name="a1">線分aの始点</param>
		/// <param name="a2">線分aの終点</param>
		/// <param name="b1">線分bの始点</param>
		/// <param name="b2">線分bの終点</param>
		/// <returns>見つかった場合は交点</returns>
		public static Vector? Intersect(Vector a1, Vector a2, Vector b1, Vector b2)
		{
			if (!IsIntersect(a1, a2, b1, b2))
			{
				return null;
			}

			var a = a2 - a1;
			var b = b2 - b1;
			return a1 + a * CrossProduct(b, b1 - a1) / CrossProduct(b, a);
		}

		/// <summary>
		/// 現在のデスクトップ上の、有効なウインドウの位置とサイズを取得します。
		/// </summary>
		/// <returns>位置とサイズのリスト</returns>
		public static IReadOnlyList<Rect> GetValidWindowRects()
		{
			return NativeMethods.EnumerateWindowHandles().
				Where(NativeMethods.IsValidWindow).
				Select(NativeMethods.GetWindowRectangle).
				Where(rect => !rect.IsEmpty && (rect.Width >= 1) && (rect.Height >= 1)).
				ToList();
		}

		/// <summary>
		/// 現在位置と次の位置で示される線分から、着地地点を取得します。
		/// </summary>
		/// <param name="boxes">着地可能な矩形のリスト</param>
		/// <param name="currentPoint">現在位置</param>
		/// <param name="nextPoint">次の位置</param>
		/// <returns>着地地点情報</returns>
		public static LandingInformation ComputeLanding(IEnumerable<Rect> boxes, Point currentPoint, Point nextPoint)
		{
			var a1 = currentPoint.ToVector();
			var a2 = nextPoint.ToVector();

			return boxes.
				OrderBy(box => box.Y).
				Select(box => new {box, p = Intersect(a1, a2, box.TopLeft.ToVector(), box.TopRight.ToVector())}).
				Where(result => result.p.HasValue).
				Select(result => new LandingInformation(result.box, result.p.Value.ToPoint())).
				FirstOrDefault();
		}

		/// <summary>
		/// 指定されたURIのイメージをロードします。
		/// </summary>
		/// <param name="uri">URI</param>
		/// <returns>ImageSource</returns>
		public static ImageSource LoadImage(string uri)
		{
			// Freezeすると、パフォーマンスが向上します。
			// （但し変更を加えることが出来なくなる。イメージ的には無問題）
			// しかし、Freezeするには、イメージのロードが同期的に完了していなければならないので、
			// BeginInitとEndInitで挟まれた内部でUriSourceを設定することでロードを実行している。
			var bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.UriSource = new Uri(uri, UriKind.RelativeOrAbsolute);
			bitmapImage.EndInit();
			bitmapImage.Freeze();
			return bitmapImage;
		}
	}
}
