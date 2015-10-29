CC=pdflatex
BIB=bibtex
MAIN=Bachelorarbeit
GLOS=makeglossaries

all:
	$(CC) $(MAIN)
	$(BIB) $(MAIN)
	$(GLOS) -g $(MAIN).aux
	$(CC) $(MAIN)
	$(CC) $(MAIN)

pdf:
	$(CC) $(MAIN)

bibtex:
	$(BIB) $(MAIN)

glossar:
	$(GLOS) $(MAIN)

clean:
	rm *.log *.lof *.aux *.out *.dvi *.bbl *.toc *.lot *.glg *.glo *.gls *.glsdefs *.ist *.acn *.acr *.alg *.blg
