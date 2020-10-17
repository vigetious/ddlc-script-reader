#DDLC Script Reader
##This area is still WIP

This is unreliable under Windows due to Python paths. I would **highly** recommend running this under the Ubuntu subsystem if you must use Windows, or forcibly changing the path of the Python environment in the config.

### Config ###
Most users will not need to change the configuration of the 

### Current commands:
``init`` - must be run on the first time to create Python environments and other configuration files.

``-wpm <number>`` -  set the word per minute. Default is 250.

``-dir <directory>`` - the directory of the mod you want to be read.

``-rm soft`` - remove all fields/folders created by this program, keeping script backups. This should be ran first if you encounter an error.

``-rm hard`` - remove all files/folders created by this program, including script backups. Should only be used as a last resort if the above command does not fix the error.


### Example Usage

``./script-reader /myDDLCMod``

``./script-reader -wpm 150 -dir /myDDLCMod``

``./script-reader -rm soft``