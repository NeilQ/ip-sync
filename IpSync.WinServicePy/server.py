#!/usr/bin/env python
# encoding: utf-8


from json import load
from urllib2 import urlopen
import redis

redis_host = '144.168.57.112'
redis_port = '6380'
redis_password = 'P@ssw0rd'
redis_db = 0

ip_request_channel = 'ip_sync_request'
ip_response_channel = 'ip_sync_response'


r = redis.StrictRedis(host=redis_host, port=redis_port, db=redis_db,password=redis_password)
p = r.pubsub(ignore_subscribe_messages=True)

def request_handler(message):
    my_ip = load(urlopen('http://jsonip.com'))['ip']
    r.publish(ip_response_channel,my_ip)

def main():
    p.subscribe(**{ip_request_channel: request_handler})
    thread = p.run_in_thread(sleep_time=0.001)
    # the event loop is now running in the background processing messages

    return


if __name__ == '__main__':
    main()