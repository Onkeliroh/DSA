MAKESolution=mdtool build
Solution = Code/Starter/.csproj
Documentation = Documentation/Doxyfile
BIN = Packages/

all:
	mdtool build $(Solution)

doc:
	doxygen $(Documentation)

clean:
	rm $(BIN)/*
