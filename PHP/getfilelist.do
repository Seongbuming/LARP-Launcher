<?php
$rpath = 'home/'.$_GET['type'];
$rarr = get_file_list($rpath);
for($i = 0; $i < sizeof($rarr); $i++)
{
	$filename = substr($rarr[$i], strlen($rpath)+1);
	$filehash = hash_file('md5', './home/'.$_GET['type'].'/'.$filename);
	echo str_replace('/','\\', $filename).','.$filehash;
	if($i != sizeof($rarr)-1)
		echo '|';
}

function get_file_list($path, $arr=array())
{
	$dir = opendir($path);
	while($file = readdir($dir))
	{
		if($file == '.' || $file == '..')
			continue;
		else if(is_dir($path.'/'.$file))
			$arr = get_file_list($path.'/'.$file, $arr);
		else
			$arr[] = $path.'/'.$file;
	}
	closedir($dir);
	return $arr;
}
?>