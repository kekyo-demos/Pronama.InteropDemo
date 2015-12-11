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
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Pronama.InteropDemo.Internals;
using Pronama.InteropDemo.StateMachines;
using Pronama.InteropDemo.UI;

namespace Pronama.InteropDemo
{
	/// <summary>
	/// KureiKeiWindowに対応するビューモデルクラスです。
	/// </summary>
	/// <remarks>
	/// ビューモデルを使用すると、ビュー（この場合はWindowクラス）から処理を分離して、独立したクラスとして定義できます。
	/// WPFの場合は、基本的にこの処理方法を使用し、Windows FormsのようにWindowクラスやControlクラスには処理を書きません。
	/// （WPFの場合は、ビューモデルを使用しないと実現できない機能があります）
	/// </remarks>
	public sealed class KureiKeiViewModel
	{
		private readonly DispatcherTimer timer_;
		private KureiKeiStateMachine stateMachine_;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public KureiKeiViewModel()
		{
			// 初期状態を保存する
			stateMachine_ = KureiKeiStateMachine.Start();
			this.Update();

			// タイマーを初期化します（まだ開始しない）
			timer_ = new DispatcherTimer(
				TimeSpan.FromMilliseconds(150),
				DispatcherPriority.Normal,
				this.OnInterval,
				Dispatcher.CurrentDispatcher);

			// Loadedイベントによってタイマーがスタートするようにします
			this.Loaded = new Command(() => timer_.Start());

			// Closedイベントによってタイマーが停止するようにします
			this.Closed = new Command(() => timer_.Stop());
		}

		/// <summary>
		/// Loadedイベントを受け取るコマンドです。
		/// </summary>
		public Command Loaded { get; private set; }

		/// <summary>
		/// Closedイベントを受け取るコマンドです。
		/// </summary>
		public Command Closed { get; private set; }

		/// <summary>
		/// 現在の座標です。
		/// </summary>
		public Property<Rect> CurrentBound { get; } = new Property<Rect>();

		/// <summary>
		/// 現在のイメージです。
		/// </summary>
		public Property<ImageSource> CurrentImage { get; } = new Property<ImageSource>();

		/// <summary>
		/// 座標とイメージを更新します。
		/// </summary>
		private void Update()
		{
			// イメージを2倍サイズにする
			var imageSize = new Size(
				stateMachine_.CurrentImage.Width * 2,
				stateMachine_.CurrentImage.Height * 2);

			// ステートマシンの位置は足元で計算しているので、頭上に変更する
			var adjustPoint = new Point(
				stateMachine_.CurrentPoint.X,
				stateMachine_.CurrentPoint.Y - imageSize.Height);

			this.CurrentBound.SetValue(new Rect(adjustPoint, imageSize));
			this.CurrentImage.SetValue(stateMachine_.CurrentImage);
		}

		/// <summary>
		/// タイマーのインターバル時間が経過したときに呼び出されるハンドラです。
		/// </summary>
		/// <param name="sender">送信元（タイマー）</param>
		/// <param name="e">イベント情報（ダミー）</param>
		private void OnInterval(object sender, EventArgs e)
		{
			// 次のステートに変更する
			stateMachine_ = stateMachine_.Next();
			this.Update();
		}
	}
}
