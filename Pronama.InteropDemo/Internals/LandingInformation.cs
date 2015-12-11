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

namespace Pronama.InteropDemo.Internals
{
	/// <summary>
	/// 着地点情報を格納するクラスです。
	/// </summary>
	public sealed class LandingInformation
	{
		public readonly Rect BoxRect;
		public readonly Point LandingPoint;

		/// <summary>
		/// コンストラクタです。
		/// </summary>
		/// <param name="boxRect">着地する矩形</param>
		/// <param name="landingPoint">着地位置</param>
		public LandingInformation(Rect boxRect, Point landingPoint)
		{
			this.BoxRect = boxRect;
			this.LandingPoint = landingPoint;
		}
	}
}
