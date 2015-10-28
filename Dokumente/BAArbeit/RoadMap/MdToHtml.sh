mdFileName="roadmap.md"
xhtmlFileName="roadmap.xhtml"

#remove exsisting file
if [ -f $mdFileName ];
then
  rm $xhtmlFileName
fi

touch $xhtmlFileName
echo "<meta charset=\"UTF-8\">" > $xhtmlFileName
markdown $mdFileName >> $xhtmlFileName
