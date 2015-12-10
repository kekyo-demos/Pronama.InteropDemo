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
using System.Runtime.InteropServices;
using System.Windows;

namespace Pronama.InteropDemo
{
	/// <summary>
	/// Win32 APIへのアクセスをカプセル化したクラスです。
	/// </summary>
	/// <remarks>
	/// Win32 APIへのアクセスを実行するための定義は、"NativeMethods"という慣例のクラス名を付けることになっています。（強制ではない）</remarks>
	public static class NativeMethods
	{
		/// <summary>
		/// Win32 APIで使用するRECT構造体の、.NET表現です。
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left;        // x position of upper-left corner
			public int Top;         // y position of upper-left corner
			public int Right;       // x position of lower-right corner
			public int Bottom;      // y position of lower-right corner
		}

		/// <summary>
		/// EnumWindows APIの第一引数に指定するコールバックに対応する、.NETのデリゲート定義です。
		/// </summary>
		/// <param name="hWnd">見つかったウインドウハンドル</param>
		/// <param name="lParam">ステート値</param>
		/// <returns>列挙を継続させる場合はtrue</returns>
		private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

		/// <summary>
		/// EnumWindows APIを示す定義です。
		/// </summary>
		/// <param name="lpEnumFunc">コールバックデリゲート</param>
		/// <param name="lParam">ステート値</param>
		/// <returns>成功すればtrue</returns>
		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

		/// <summary>
		/// IsWindowVisible APIを示す定義です。
		/// </summary>
		/// <param name="hWnd">ウインドウハンドル</param>
		/// <returns>ウインドウが可視であるならtrue</returns>
		[DllImport("user32.dll")]
		private static extern bool IsWindowVisible(IntPtr hWnd);

		/// <summary>
		/// GetWindowRect APIを示す定義です。
		/// </summary>
		/// <param name="hwnd">ウインドウハンドル</param>
		/// <param name="lpRect">ウインドウの位置とサイズを示すRECT構造体（outパラメータなので、APIにはポインタとして渡される）</param>
		/// <returns>成功すればtrue</returns>
		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

		/// <summary>
		/// 現在デスクトップ領域に存在する、すべてのウインドウハンドルを全て列挙します。
		/// </summary>
		/// <returns>ウインドウハンドル群</returns>
		public static IReadOnlyList<IntPtr> EnumerateWindowHandles()
		{
			var list = new List<IntPtr>();
			if (EnumWindows((hWnd, lParam) =>
				{
					list.Add(hWnd);
					return true;
				},
				IntPtr.Zero) == false)
			{
				// エラーを例外に変換
				Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			}

			return list;
		}

		/// <summary>
		/// 指定されたウインドウハンドルのウインドウが有効かどうかを取得します。
		/// </summary>
		/// <param name="window">ウインドウハンドル</param>
		/// <returns>有効ならtrue</returns>
		public static bool IsValidWindow(IntPtr window)
		{
			return IsWindowVisible(window);
		}

		/// <summary>
		/// 指定されたウインドウハンドルのウインドウの位置とサイズを取得します。
		/// </summary>
		/// <param name="window">ウインドウハンドル</param>
		/// <returns>位置とサイズを示すRect</returns>
		public static Rect GetWindowRectangle(IntPtr window)
		{
			// GetWindowRectを呼び出してRECTを取得する
			RECT rect;
			if (GetWindowRect(window, out rect) == false)
			{
				// エラーを例外に変換
				Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			}

			// RECT構造体のままでも使えなくはないが扱いづらいので、WPFのRectクラスに入れ替えて返す

			// サイズが0のウインドウはEmptyとして返す
			if ((rect.Left == 0) && (rect.Right == 0) && (rect.Top == 0) && (rect.Bottom == 0))
			{
				return Rect.Empty;
			}

			// サイズを計算して返す
			return new Rect(rect.Left, rect.Top, rect.Right - rect.Left + 1, rect.Bottom - rect.Top + 1);
		}
	}
}
