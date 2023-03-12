; Яцков Максим КВ-13

; TASM.EXE /zi /c /l test2.asm
; Ідентифікатори
; Містять великі і малі букви латинского алфавіту та цифри. Починаються з букви. Великі та малі букви не відрізняються. 
; Константи 
; Шістнадцятерічні, десяткові та двійкові константи
; Директиви
; END,
; model {tiny small}
; .data., .code 
; DB,DW,DD з одним операндом - константою
; Розрядність даних та адрес
; 16 - розрядні дані та зміщення в сегменті, в випадку 32-розрядних даних та 32 розрядних зміщень генеруються відповідні префікси зміни розрядності
; Адресація операндів пам'яті
; Індексна адресація (Val1[eax+edx],Val1[ecx+edi] і т.п.)
; Заміна сегментів
; Префікси заміни сегментів можуть задаватись явно, а при необхідності автоматично генеруюься транслятором.
; Машинні команди
; Sahf
; Sal reg,1
; Rcr mem,1
; Sbb reg,reg
; Test reg,mem
; Bts mem,reg
; Mov reg,imm
; Adc mem,imm
; Jnb
; jmp (внутрішньосегментна відносна адресація)
; Де reg – 8,16 або 32-розрядні РЗП
; mem – адреса операнда в пам’яті
; imm - 8,16 або 32-розрядні безпосередні дані (константи)

; .486p
.model tiny
.data
	varb    db  10
	varw    dw  00a2h
	vard    dd  1000011b

.code
	jmp start
l1:
	jmp exit

start:
	sahf

    sal ax, 1
    sal ebx, 1

	rcr WORD ptr varw[ebx+edx], 1
	rcr DWORD ptr vard[ebx+edi], 1

	sbb ax, bx
	sbb ecx, eax

	test dx, varw
	test ebx, vard

	bts varw, cx
	bts vard, eax

	mov cx, 100111b
	mov edx, 12h

	adc varw, 56d
	adc vard, 1c25h

	jnb l1
	jmp l1

exit:
	; mov ah, 4ch
    ; int 21h
end start