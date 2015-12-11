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

namespace Pronama.InteropDemo.StateMachines
{
	/// <summary>
	/// ステートマシンの基底クラスです。
	/// </summary>
	public abstract class KureiKeiStateMachine
	{
		protected KureiKeiStateMachine()
		{
		}

		/// <summary>
		/// 現在の位置を取得します。
		/// </summary>
		public Point CurrentPoint { get; protected set; }

		/// <summary>
		/// 現在のイメージを取得します。
		/// </summary>
		public ImageSource CurrentImage { get; protected set; }

		/// <summary>
		/// 次のステートを計算し、ステートマシンを取得します。
		/// </summary>
		/// <returns>次のステートマシン</returns>
		public abstract KureiKeiStateMachine Next();
	}
}
