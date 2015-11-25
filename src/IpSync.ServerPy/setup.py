#!/usr/bin/env python
# encoding: utf-8

from setuptools import setup

setup(name='ip-sync',
      version='0.1.0',
      description='public ip sync tool with dropbox',
      author='neilq',
      author_email='neil_chen_0918@live.cn',
      url='https://github.com/neilq/ip-sync',
      packages=['server_dropbox'],
      entry_points={
          'console_scripts': [
              'ipsync = server_dropbox.__main__:main',
          ]
      }
     )