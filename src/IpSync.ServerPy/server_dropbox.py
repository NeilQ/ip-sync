#!/usr/bin/env python
# encoding: utf-8

import os
from json import load
from urllib2 import urlopen, HTTPError
from datetime import datetime
import thread
import socket
import time


my_ip = ''
dropbox_path = 'D:\\Dropbox\\'
ips_folder = dropbox_path + 'ip_sync-%s\\' % (socket.gethostname())
result_filename = 'ip.txt'
log_filename = 'ip.log'


def init():
     # check dropbox path exists
    if not os.path.isdir(dropbox_path) :
        log('dropbox path %s not exists' % (dropbox_path))
    # create child folder 'ips'
    if not os.path.isdir(ips_folder) :
        os.makedirs(ips_folder)
    return

def pretty_time():
    return datetime.now().strftime('%Y-%m-%d %H:%M:%S')

def query_ip():
    try:
        ip = load(urlopen('http://jsonip.com'))['ip']
    except HTTPError :
        log('Fail to fetch ip, please check you internet access.')
        return
    return ip 

def log(str):
    if not str: return
    print(str)
    logf = open(ips_folder + log_filename, 'a')
    logf.write('%s%s%s\n' % (pretty_time(), ' ' * 4, str))
    logf.close()
    
def sync_ip(delay):
    global my_ip
    while True:
        new_ip = query_ip()
        if my_ip is new_ip:
            time.sleep(delay)
            continue 
        my_ip = new_ip

        #write to file
        log('ip: %s' % (my_ip))
        f = open(ips_folder + result_filename,'w')
        f.write('%s\n%s\n' % (my_ip, pretty_time()))
        f.close()
        time.sleep(delay)

def main():
    import argparse
    global dropbox_path, ips_folder
    delay = 30
    parser = argparse.ArgumentParser()
    parser.add_argument('-f', '--folder', help='The folder path of the dropbox.')
    parser.add_argument('-d', '--delay',type=int, help='The time interval to fetch ip.(seconds)')
    args = parser.parse_args()
    if args.folder:
        dropbox_path = args.folder + '\\'
        ips_folder = dropbox_path + 'ip_sync-%s\\' % (socket.gethostname())
    if args.delay:
        delay = args.delay

    init()
    #Timer(interval,sync_ip).start()
    thread.start_new_thread(sync_ip, (delay,))
    log('starting...')


if __name__ == '__main__':
    main()