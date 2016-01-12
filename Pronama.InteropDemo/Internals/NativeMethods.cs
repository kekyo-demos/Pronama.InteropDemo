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

namespace Pronama.InteropDemo.Internals
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

		private static Rect ToRect(this RECT rect)
		{
			// サイズが0のウインドウはEmptyとして返す
			if ((rect.Left == 0) && (rect.Right == 0) && (rect.Top == 0) && (rect.Bottom == 0))
			{
				return Rect.Empty;
			}

			// サイズを計算して返す
			return new Rect(rect.Left, rect.Top, rect.Right - rect.Left + 1, rect.Bottom - rect.Top + 1);
		}

		#region GetWindowRectangle
		/// <summary>
		/// GetWindowRect APIを示す定義です。
		/// </summary>
		/// <param name="hwnd">ウインドウハンドル</param>
		/// <param name="lpRect">ウインドウの位置とサイズを示すRECT構造体（outパラメータなので、APIにはポインタとして渡される）</param>
		/// <returns>成功すればtrue</returns>
		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

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
			return rect.ToRect();
		}
		#endregion

		#region SetWindowRectangle
		private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
		private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
		private static readonly IntPtr HWND_TOP = new IntPtr(0);
		private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

		/// <summary>
		/// SetWindowPos Flags
		/// </summary>
		[Flags]
		private enum SetWindowPosFlags
		{
			NOSIZE = 0x0001,
			NOMOVE = 0x0002,
			NOZORDER = 0x0004,
			NOREDRAW = 0x0008,
			NOACTIVATE = 0x0010,
			DRAWFRAME = 0x0020,
			FRAMECHANGED = 0x0020,
			SHOWWINDOW = 0x0040,
			HIDEWINDOW = 0x0080,
			NOCOPYBITS = 0x0100,
			NOOWNERZORDER = 0x0200,
			NOREPOSITION = 0x0200,
			NOSENDCHANGING = 0x0400,
			DEFERERASE = 0x2000,
			ASYNCWINDOWPOS = 0x4000
		}

		/// <summary>
		/// SetWindowPos APIを示す定義です。
		/// </summary>
		/// <param name="hWnd">ウインドウハンドル</param>
		/// <param name="hWndInsertAfter">挿入目標位置を示すウインドウハンドル又は特殊なフラグ</param>
		/// <param name="X">X座標</param>
		/// <param name="Y">Y座標</param>
		/// <param name="cx">幅</param>
		/// <param name="cy">高さ</param>
		/// <param name="uFlags">オプションフラグ</param>
		/// <returns>成功すればtrue</returns>
		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool SetWindowPos(
			IntPtr hWnd, IntPtr hWndInsertAfter,
			int X, int Y, int cx, int cy,
			SetWindowPosFlags uFlags);

		/// <summary>
		/// 指定されたウインドウハンドルのウインドウの位置とサイズを設定します。
		/// </summary>
		/// <param name="window">ウインドウハンドル</param>
		/// <param name="rect">矩形</param>
		public static void SetWindowRectangle(IntPtr window, Rect rect)
		{
			if (SetWindowPos(
				window,
				HWND_TOP,	// 手前に表示
				(int)rect.X, (int)rect.Y,
				(int)rect.Width, (int)rect.Height,
				SetWindowPosFlags.NOZORDER | SetWindowPosFlags.NOSIZE | SetWindowPosFlags.NOACTIVATE) == false)
			{
				// エラーを例外に変換
				Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
		}
		#endregion

		#region GetDesktopRectangle
		private const uint SPI_GETWORKAREA = 0x0030;

		/// <summary>
		/// SystemParametersInfo API（但しRECT構造体のみ対応）を示す定義です。
		/// </summary>
		/// <param name="uiAction">種類</param>
		/// <param name="uiParam">パラメータ（未使用）</param>
		/// <param name="pvParam">RECT構造体</param>
		/// <param name="fWinIni">取得操作フラグ（未使用）</param>
		/// <returns></returns>
		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool SystemParametersInfo(
			uint uiAction, uint uiParam, ref RECT pvParam, uint fWinIni);

		/// <summary>
		/// デスクトップ領域の矩形を取得します。
		/// </summary>
		/// <returns>デスクトップの矩形</returns>
		public static Rect GetDesktopRectangle()
		{
			// TODO: この情報は、マルチモニターに対応していません。

			var rect = new RECT();
			if (SystemParametersInfo(SPI_GETWORKAREA, 0, ref rect, 0) == false)
			{
				// エラーを例外に変換
				Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			}

			return rect.ToRect();
		}
		#endregion
	}
}
