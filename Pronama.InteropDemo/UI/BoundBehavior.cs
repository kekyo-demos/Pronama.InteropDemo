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
using System.Windows.Interactivity;
using System.Windows.Interop;
using Pronama.InteropDemo.Internals;

namespace Pronama.InteropDemo.UI
{
	/// <summary>
	/// ウインドウの位置・サイズを指定する矩形をバインディング可能にするクラスです。
	/// </summary>
	/// <remarks>
	/// WPF WindowクラスのLeftとTopプロパティは依存関係プロパティですが、
	/// バインディングが正しく機能しません。
	/// このビヘイビアを使用すると、内部でWin32 APIを使用して、
	/// バインディングでウインドウ矩形を矯正することが出来ます。
	/// </remarks>
	public sealed class BoundBehavior : Behavior<Window>
	{
		/// <summary>
		/// ウインドウの矩形を示す依存関係プロパティです。
		/// </summary>
		public static readonly DependencyProperty BoundProperty =
			DependencyProperty.Register("Bound", typeof(Rect), typeof(BoundBehavior),
				new UIPropertyMetadata(default(Rect), BoundChanged));

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public BoundBehavior()
		{
		}

		/// <summary>
		/// ウインドウの矩形を示します。
		/// </summary>
		/// <remarks>
		/// ユーザーによって操作されるウインドウ矩形の状態は反映されません。
		/// </remarks>
		public Rect Bound
		{
			get
			{
				return (Rect)base.GetValue(BoundProperty);
			}
			set
			{
				base.SetValue(BoundProperty, value);
			}
		}

		/// <summary>
		/// ビヘイビアがアタッチされる際に呼び出されます。
		/// </summary>
		protected override void OnAttached()
		{
			base.OnAttached();

			// ウインドウ矩形を設定するにはウインドウハンドルが必要なので、
			// まだ存在しなければここで生成する
			var wih = new WindowInteropHelper(base.AssociatedObject);
			wih.EnsureHandle();

			// 現在の値で矩形を設定する
			NativeMethods.SetWindowRectangle(wih.Handle, this.Bound);
		}

		/// <summary>
		/// Boundプロパティが変更された際に呼び出されます。
		/// </summary>
		/// <param name="e">変更情報</param>
		private void BoundChanged(DependencyPropertyChangedEventArgs e)
		{
			if (base.AssociatedObject != null)
			{
				var wih = new WindowInteropHelper(base.AssociatedObject);
				wih.EnsureHandle();

				NativeMethods.SetWindowRectangle(wih.Handle, this.Bound);
			}
		}

		/// <summary>
		/// Boundプロパティが変更された際に呼び出されます。
		/// </summary>
		/// <param name="d">対象のインスタンス</param>
		/// <param name="e">変更情報</param>
		private static void BoundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var setLocationBehavior = (BoundBehavior) d;
			setLocationBehavior.BoundChanged(e);
		}
	}
}
