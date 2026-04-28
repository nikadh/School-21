
if [ "$#" -ne 3 ]; then
echo "Неверное количество аргументов"
exit 1
fi
path=$1
old=$2
new=$3
cd ..
if [ ! -f "$path" ]; then
echo "Файла нет"
exit 1
fi
if [ "$old" == "" ]; then
echo "Пустая строка" 
exit 1
fi
sed -i '' "s/$old/$new/g" $path
echo "All good"
size=$(wc -c "$path" | awk '{print $1}')
sha=$(shasum -a 256 $path | awk '{print $1}')
date=$(date +"%Y-%m-%d %H:%M")
echo "$path - $size - $date - $sha - sha256" >> "src/files.log"
exit 0
