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

using System.Linq;
using System.Windows;
using System.Windows.Media;
using Pronama.InteropDemo.Internals;

namespace Pronama.InteropDemo.StateMachines
{
	/// <summary>
	/// 歩行中を示すステートマシンです。
	/// </summary>
	public sealed class KureiKeiWalkingStateMachine : KureiKeiStateMachine
	{
		private static ImageSource[] walkingImages_;

		private readonly LandingInformation landingInformation_;
		private int walkingImageIndex_ = 0;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		/// <param name="landingInformation">着地位置情報</param>
		public KureiKeiWalkingStateMachine(LandingInformation landingInformation)
		{
			if (walkingImages_ == null)
			{
				// 歩いているイメージ群をロードします
				walkingImages_ = Enumerable.Range(1, 6).
					Select(index => Utilities.LoadImage(string.Format("pack://application:,,,/Images/01-{0}.png", index))).
					ToArray();
			}

			landingInformation_ = landingInformation;

			base.CurrentPoint = landingInformation_.LandingPoint;
			base.CurrentImage = walkingImages_[walkingImageIndex_++];
		}

		/// <summary>
		/// 次のステートを計算し、ステートマシンを取得します。
		/// </summary>
		/// <returns>次のステートマシン</returns>
		public override KureiKeiStateMachine Next()
		{
			// TODO:ウインドウの変化を見ていない
			//   ヒント:ウインドウの変化を見るためには、乗っているウインドウがどれかを監視する必要があります。
			//     今はLandingInformationにその情報がありません。ウインドウハンドルを使って識別するのが良いでしょう。

			// 24pxづつ左に移動する
			base.CurrentPoint = new Point(base.CurrentPoint.X - 24, base.CurrentPoint.Y);

			// イメージを進める
			base.CurrentImage = walkingImages_[walkingImageIndex_++];

			// 0 --> 1 --> 2 --> 3 --> 4 --> 5 を繰り返す
			walkingImageIndex_ = walkingImageIndex_ % walkingImages_.Length;

			// 矩形の左端まで来た
			if (base.CurrentPoint.X < landingInformation_.BoxRect.X)
			{
				// 見えないところまで来た
				if (base.CurrentPoint.X < -64)
				{
					// 最初の地点に戻す
					return KureiKeiStateMachine.Start();
				}
				else
				{
					// 左端まで来たら、落下ステートに変更する
					return new KureiKeiFallStateMachine(base.CurrentPoint);
				}
			}

			// 今のステートを繰り返す
			return this;
		}
	}
}
