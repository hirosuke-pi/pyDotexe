# -*- encoding:utf-8 -*-
from setuptools import setup, find_packages

setup(
    name='pydotexe',
    version='2.1.2',
    author='betacode_',
    author_email='betacode.net@gmail.com',
    description = 'pyDotexe is a program that freezes Python programs into stand-alone executables under Windows.',
    long_description = '',
    url = 'https://github.com/betacode-project/pyDotexe',
    license = 'MIT',
    packages=find_packages(),    
    platforms = ['Windows'],
    classifiers=[
        "Environment :: Console",
        "Intended Audience :: Developers",
        "License :: OSI Approved :: MIT License",
        "Operating System :: Microsoft :: Windows",
        "Programming Language :: Python :: 2",
        "Programming Language :: Python :: 2.7",
        "Programming Language :: Python :: 3",
        "Topic :: Software Development",
        "Topic :: Software Development :: Build Tools"
    ]
)