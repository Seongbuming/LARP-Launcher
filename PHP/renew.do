<?php
require_once('security/encryption.inc');
$config = parse_ini_file('security/config.ini');

$db = new mysqli($config['db']['host'], $config['db']['user'], $config['db']['password'], $config['db']['database']);
if ($db->connect_error)
    die('0.1');
$db->query("SET SESSION CHARACTER_SET_CONNECTION = UTF8");
$db->query("SET SESSION CHARACTER_SET_RESULTS = UTF8");
$db->query("SET SESSION CHARACTER_SET_CLIENT = UTF8");

$data = json_decode(decrypt($_POST['data'], $config['encrypt']['launcher_key']), true);
$tdata = json_decode(decrypt($data['TToken'], $config['encrypt']['token_key']), true);
$cdata = json_decode(decrypt($data['CToken'], $config['encrypt']['token_key']), true);

if ($data['Username'] != $tdata['Username'] || $data['Username'] != $cdata['Username'])
    die('0.2');
if ($tdata['IP'] != $cdata['IP'])
    die('0.3');
if ($tdata['Publisher'] != $config['token_publisher']['freq'])
    die('0.4');
if ($cdata['Publisher'] != $config['token_publisher']['conn'])
    die('0.5');

// TToken 유효 검사
$ttokenID = intval($tdata['ID']);
$ttoken = $db->escape_string($data['TToken']);
$query = $db->query("
    SELECT ID
    FROM launcher_user_token_data
    WHERE
        Type = 'freq'
        AND ID = $ttokenID
        AND Token = '$ttoken'
        AND TIMESTAMPDIFF(MINUTE, PublishedTime, NOW()) <= 3");
if ($query->num_rows == 0)
    die('0.6');

// CToken 유효 검사
$ctokenID = intval($cdata['ID']);
$ctoken = $db->escape_string($data['CToken']);
$query = $db->query("
    SELECT ID
    FROM launcher_user_token_data
    WHERE
        Type = 'conn'
        AND ID = $ctokenID
        AND Token = '$ctoken'
        AND TIMESTAMPDIFF(MINUTE, RenewedTime, NOW()) <= 3");
if ($query->num_rows == 0)
    die('0.7');

// CToken 토큰 갱신
$db->query("
    UPDATE launcher_user_token_data SET
        RenewedTime = CURRENT_TIMESTAMP
    WHERE
        Type = 'conn'
        AND ID = $ctokenID");
print('1');
?>
