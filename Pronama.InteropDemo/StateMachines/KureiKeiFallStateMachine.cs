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

using System.Windows;
using System.Windows.Media;
using Pronama.InteropDemo.Internals;

namespace Pronama.InteropDemo.StateMachines
{
	/// <summary>
	/// 落下中を示すステートマシンです。
	/// </summary>
	public sealed class KureiKeiFallStateMachine : KureiKeiStateMachine
	{
		private static ImageSource fallImage_;
		private int accelleration_ = 1;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		/// <param name="startPoint">落下開始位置</param>
		public KureiKeiFallStateMachine(Point startPoint)
		{
			if (fallImage_ == null)
			{
				// 落ちているイメージをロードします
				fallImage_ = Utilities.LoadImage("pack://application:,,,/Images/06-A.png");
			}

			base.CurrentPoint = startPoint;
			base.CurrentImage = fallImage_;
		}

		/// <summary>
		/// 次のステートを計算し、ステートマシンを取得します。
		/// </summary>
		/// <returns>次のステートマシン</returns>
		public override KureiKeiStateMachine Next()
		{
			// 今回の落下位置
			var nextPoint = new Point(base.CurrentPoint.X - 24, base.CurrentPoint.Y + 4 * accelleration_);
			accelleration_++;

			// デスクトップ上の全ての可視ウインドウを取得
			var boxes = Utilities.GetValidWindowRects();

			// 着地点を計算
			var landingInformation = Utilities.ComputeLanding(boxes, base.CurrentPoint, nextPoint);
			if (landingInformation != null)
			{
				// 歩行ステートに変更する
				return new KureiKeiWalkingStateMachine(landingInformation);
			}

			// 着地しなかった
			base.CurrentPoint = nextPoint;

			// 見えないところまで来た
			if (base.CurrentPoint.X < -64)
			{
				// 最初の地点に戻す
				return KureiKeiStateMachine.Start();
			}

			return this;
		}
	}
}
