// cpp00.cpp : このファイルには 'main' 関数が含まれています。プログラム実行の開始と終了がそこで行われます。
//

#include "stdafx.h"

BOOL EnumFunc(HWND hWnd, LPARAM lp)
{
	const LRESULT ret = ::SendMessageTimeout(hWnd, WM_FONTCHANGE, 0, 0, SMTO_ABORTIFHUNG, 1000, nullptr);
	if (!ret)
	{
		int n = 0;
	}
	return true;
}

int main()
{
	::EnumWindows(EnumFunc, 0);
	return 0;
}


