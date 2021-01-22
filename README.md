# Ren'Py Tools

This application is still in its infancy, and as such only has limited functionality.

At the moment, the application will tell you how many words are in your game, and how long it will take to play (using a default WPM of 250).

It can also extract all of the script from a game, exposing all spoken words by characters.

### Setup

Download the program and move it to an appropriate directory.

If you are on a Unix-based system, don't forget to ``chmod +x ./renpy-tools`` to make it an executable.

### Usage

Although this has been tested in Windows, more time and effort has been put into testing in a Unix environment. Because of this, you are more likely to find less bugs using Unix. If you are able to, please use Unix.

Before running the program, please make sure you have all of the dependencies listed at the bottom of the page. 

Always run ``init`` first. This downloads and installs all of the Python dependencies and creates configuration files, all necessary to run the program.

After running ``init``, provide either a directory or compressed file. Currently supported formats are:
- Zip
- GZip
- BZip2
- Tar
- Rar
- LZip
- XZ

The following are the optional commands.

### Current commands

| Command              | Description                                                                                                                                                   |
|----------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------|
| ``init``             | Must be run on the first time to create Python environments and other configuration files.                                                                    |
| ``-wpm <number>``    | Set the word per minute. Default is 250.                                                                                                                      |
| ``-dir <directory>`` | The directory or compressed file of the mod you want to be read.                                                                                              |
| ``-rm soft``         | Remove all fields/folders created by this program, keeping script backups. This should be ran first if you encounter an error.                                |
| ``-rm hard``         | Remove all files/folders created by this program, including script backups. Should only be used as a last resort if the above command does not fix the error. |
| ``-keepRPA``         | Keep extracted RPA files, instead of auto-deleting them after the operation runs. The files are kept in the created "extracted" directory.                    |
| ``-keepScript``      | Keep extracted script, instead of auto-deleting them after the operation runs. The files are kept int he created "backup" directory.                          |
| ``-json``            | Output in JSON format. Useful if tunneling output into another program.                                                                                       |
| ``-update``          | Check for application updates.                                                                                                                                |

### Example Usage

``./script-reader /game``

``./script-reader /game.zip``

``./script-reader -wpm 150 -dir /game``

``./script-reader -rm soft``

``./script-reader /game -keepScript -json``

### Dependencies

- Python 2
- Python 3

### Thanks

Thanks to the developers of:

[unrpyc](https://github.com/CensoredUsername/unrpyc)

[unrpa](https://github.com/Lattyware/unrpa)

This program would not be possible without their contributions to the community.
