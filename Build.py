import subprocess
import os
from os import path
from glob import glob
import shutil
import sys

home_dir = path.dirname(__file__)

if os.getcwd() is not home_dir:
    os.chdir(home_dir)

subprocess.call('nuget restore', shell=True)

files = glob("*.sln")

for file in files:
    subprocess.call(f'msbuild "{path.abspath(file)}" -p:Configuration=Release')

os.chdir("EditorCorePlus\\bin\\Release\\netstandard2.0")

subprocess.call(f'7z a "{home_dir}\\EditorCorePlus.zip" *.dll -r')

os.chdir(f"{home_dir}\\EditorCorePlus\\bin\\Release")

files = glob('*.nupkg')

for file in files:
    shutil.move(path.abspath(file), f'{home_dir}\\{file}')

os.chdir(home_dir)

sys.exit(0)