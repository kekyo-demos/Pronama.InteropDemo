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
using System.Windows.Input;

namespace Pronama.InteropDemo
{
	/// <summary>
	/// XAMLデータバインディングで、ビューのイベントによる通知をビューモデルに通知することが出来る、簡易的なヘルパークラスです。
	/// </summary>
	/// <typeparam name="TParameter">イベントに対応付けられたパラメータの型</typeparam>
	/// <remarks>
	/// このクラスを使うと、XAMLデータバインディング（ビューのイベントをビューモデルに通知）が実現します。
	/// このクラスは簡易的な実装です。本格的にはPrism・MVVMLight・ReactiveProperty等を使用してください。
	/// </remarks>
	public class Command<TParameter> : ICommand
	{
		private readonly Action<TParameter> action_;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		/// <param name="action">通知されると呼び出されるデリゲート</param>
		/// <remarks>
		/// ビュー側（XAML定義）でイベントが発生すると、最終的にこのactionデリゲートを呼び出します。
		/// </remarks>
		public Command(Action<TParameter> action)
		{
			action_ = action;
		}

		/// <summary>
		/// 実行可能状態を通知するイベントです。
		/// </summary>
		/// <remarks>
		/// このイベントはWPFが使用しますが、内部から発火することはありません。
		/// </remarks>
		event EventHandler ICommand.CanExecuteChanged
		{
			add { }
			remove { }
		}

		/// <summary>
		/// 実行可能かどうかを確認します。
		/// </summary>
		/// <param name="parameter">パラメータ</param>
		/// <returns>実行可能ならtrue</returns>
		/// <remarks>
		/// このメソッドはWPFが呼び出します。
		/// このクラスは常に実行可能であるため、trueを返します。
		/// </remarks>
		bool ICommand.CanExecute(object parameter)
		{
			return true;
		}

		/// <summary>
		/// ビューモデルのハンドラを実行します。
		/// </summary>
		/// <param name="parameter"></param>
		/// <remarks>
		/// このメソッドはWPFが呼び出します。
		/// イベントが発火した際に呼び出され、actionデリゲートにバイパスします。
		/// </remarks>
		void ICommand.Execute(object parameter)
		{
			action_((TParameter)parameter);
		}
	}

	/// <summary>
	/// XAMLデータバインディングで、ビューのイベントによる通知をビューモデルに通知することが出来る、簡易的なヘルパークラスです。
	/// </summary>
	/// <remarks>
	/// このクラスは、パラメータを必要としない既定の実装です。
	/// </remarks>
	public sealed class Command : Command<object>
	{
		/// <summary>
		/// コンストラクタです。
		/// </summary>
		/// <param name="action">通知されると呼び出されるデリゲート</param>
		/// <remarks>
		/// ビュー側（XAML定義）でイベントが発生すると、最終的にこのactionデリゲートを呼び出します。
		/// </remarks>
		public Command(Action action)
			: base(e => action())
		{
		}
	}
}
