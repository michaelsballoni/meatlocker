# meetlocker
Meet Locker is a data storage and retrieval application.

## Storage
The user provides the password and the data, and the server puts the data into a ZIP file protected by the password.  The server generates a key and uses this key and the password to determine where to store the ZIP file.  The key is returned to the user.  

## Retrieval
The user provides the key and the password and the server looks for the ZIP file and returns the bytes of the ZIP file for the user to provide the password to and access the original data.  Once the client receives the data, the server deletes the ZIP file.

## Maintenance
The server periodically walks all ZIP files and deletes ones that have never accessed, or who have been accessed but not recently.

## Security
Passwords and keys are not stored. \
With just a key or just a password, you cannot find or open a ZIP file.
