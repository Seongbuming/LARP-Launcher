<?php
require_once('security/encryption.inc');

$config = parse_ini_file('security/config.ini');

$type = $_GET['type'];
if (strpos($type, '.') !== false || strpos($type, '/') !== false) {
    exit();
} else {
    $tokenPublisher = 'security/'.$config['token_publisher'][$type].'.inc';
    if (file_exists($tokenPublisher)) {
        require_once($tokenPublisher);
    }
}
?>
