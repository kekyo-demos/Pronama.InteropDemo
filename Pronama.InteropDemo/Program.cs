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
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Pronama.InteropDemo
{
	public static class Program
	{
		// メインエントリポイントです
		[STAThread]
		public static int Main(string[] args)
		{
			try
			{
				// WPFのApplicationクラスを生成し、シャットダウンモードを手動にします
				var application = new System.Windows.Application();
				application.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;

				// プロ生サイトを表示するメニューを定義します
				var pronamaMenu = new System.Windows.Forms.ToolStripMenuItem("Show &Pronama site...");
				pronamaMenu.Click += (s, e) => Process.Start("http://pronama.azurewebsites.net/");

				// CuteRun.PronamaChanサイトを表示するメニューを定義します
				var cuteRunMenu = new System.Windows.Forms.ToolStripMenuItem("Show &CuteRun.PronamaChan site...");
				cuteRunMenu.Click += (s, e) => Process.Start("https://github.com/kekyo/Pronama.InteropDemo");

				// 終了メニューを定義します
				var exitMenu = new System.Windows.Forms.ToolStripMenuItem("E&xit");
				exitMenu.Click += (s, e) => application.Shutdown(0);

				// システムトレイに表示するメニューを定義します
				var rootContextMenu = new System.Windows.Forms.ContextMenuStrip();
				rootContextMenu.Items.Add(pronamaMenu);
				rootContextMenu.Items.Add(cuteRunMenu);
				rootContextMenu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
				rootContextMenu.Items.Add(exitMenu);

				// システムトレイにアイコンを表示します
				using (var notifyIcon = new System.Windows.Forms.NotifyIcon
				{
					Icon = Properties.Resources.CuteRun_PronamaChan,
					Text = "CuteRun.PronamaChan!",
					ContextMenuStrip = rootContextMenu,
					Visible = true
				})
				{
					// プロ生ちゃんアニメーションの表示を開始します
					var kureiKeiWindow = new KureiKeiWindow();
					kureiKeiWindow.Show();

					// 終了するまで待機します
					application.Run();
				}
			}
			catch (Exception ex)
			{
				// エラーが発生したら、メッセージボックスで表示します
				System.Windows.Forms.MessageBox.Show(
					ex.Message,
					"CuteRun.PronamaChan!",
					System.Windows.Forms.MessageBoxButtons.OK,
					System.Windows.Forms.MessageBoxIcon.Error);
				return Marshal.GetHRForException(ex);
			}

			return 0;
		}
	}
}
