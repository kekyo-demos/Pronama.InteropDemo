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

using System.ComponentModel;
using System.Diagnostics;

namespace Pronama.InteropDemo.UI
{
	/// <summary>
	/// XAMLデータバインディングで、ビューモデルのプロパティをバインディング可能にする、簡易的なヘルパークラスです。
	/// </summary>
	/// <typeparam name="TValue">バインディング対象の値の型</typeparam>
	/// <remarks>
	/// このクラスを使うと、XAMLデータバインディング（ビューモデルからビューに値を自動的に転送する）が実現します。
	/// このクラスは簡易的な実装です。本格的にはPrism・MVVMLight・ReactiveProperty等を使用してください。
	/// </remarks>
	[DebuggerDisplay("{Value}")]
	public sealed class Property<TValue> : INotifyPropertyChanged
	{
		/// <summary>
		/// コンストラクタです。
		/// </summary>
		public Property()
		{
		}

		/// <summary>
		/// ビューに転送する値です。
		/// </summary>
		/// <remarks>
		/// このプロパティはXAMLからバインディング式によって参照されます。
		/// </remarks>
		public TValue Value { get; private set; }

		/// <summary>
		/// プロパティが変更されたことを示すイベントです。
		/// </summary>
		/// <remarks>
		/// このイベントはWPFが使用します。
		/// </remarks>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// 新しい値を設定します。
		/// </summary>
		/// <param name="value">値</param>
		/// <remarks>値を設定すると、値が更新されたことがWPFに通知され、ビューの表示が更新されます。</remarks>
		public void SetValue(TValue value)
		{
			if ((this.Value == null) && (value == null))
			{
				return;
			}

			if ((this.Value != null) && (value != null))
			{
				if (this.Value.Equals(value))
				{
					return;
				}
			}

			this.Value = value;

			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
		}
	}
}
