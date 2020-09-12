^space::
	Loop, parse, clipboard, `n, `r
	{
		Send, %A_LoopField%
		Send, {Enter}
	}