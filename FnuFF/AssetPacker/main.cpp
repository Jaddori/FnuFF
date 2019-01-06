#include <stdio.h>
#include <string.h>

#define MAX_INPUT_FILES 1024
#define MAX_TEXTURE_NAME_LEN 64
#define BUFFER_SIZE 1024

enum
{
	MODE_NONE = 0,
	MODE_TEXTURES,

	MODE_ARGS = 0,
	MODE_FILE,
	MODE_FOLDER,
};

int parseInputFiles( char** inputFiles, char* rootFolder, char* fileContent );
int packContent( char** inputFiles, int inputFileCount, char* outputName, int contentMode );
int packTextures( char** inputFiles, int inputFileCount, char* outputName );

int main( int argc, char* argv[] )
{
	int contentMode = MODE_NONE;
	int inputMode = MODE_ARGS;
	char* outputName = NULL;
	char* rootFolder = NULL;
	
	// parse arguments
	for( int i=1; i<argc; i++ )
	{
		// modes
		if( strcmp( argv[i], "-textures" ) == 0 )
			contentMode = MODE_TEXTURES;

		// input
		else if( strcmp( argv[i], "-file" ) == 0 )
			inputMode = MODE_FILE;

		// output file name
		else if( strcmp( argv[i], "-output" ) == 0 )
		{
			int len = strlen( argv[i+1] );
			outputName = new char[len+1];
			strcpy( outputName, argv[i+1] );
			outputName[len] = 0;

			i++;
		}

		// root folder
		else if( strcmp( argv[i], "-root" ) == 0 )
		{
			int len = strlen( argv[i+1] );
			rootFolder = new char[len+1];
			strcpy( rootFolder, argv[i+1] );
			rootFolder[len] = 0;

			i++;
		}
	}

	// get input files
	char** inputFiles = new char*[MAX_INPUT_FILES];
	int inputFileCount = 0;
	if( inputMode == MODE_ARGS )
	{
		for( int i=1; i<argc; i++ )
		{
			if( strcmp( argv[i], "-output" ) == 0 ||
				strcmp( argv[i], "-root" ) == 0 )
			{
				i++;
			}
			else if( argv[i][0] != '-' )
			{
				int len = strlen( argv[i] );
				inputFiles[inputFileCount] = new char[len+1];
				strcpy( inputFiles[inputFileCount], argv[i] );
				inputFiles[inputFileCount][len] = 0;
				inputFileCount++;
			}
		}

		if( inputFileCount < 1 )
		{
			printf( "No input files provided. Aborting.\n" );
			return 1;
		}
	}
	else if( inputMode == MODE_FILE )
	{
		int fileArgIndex = 1;
		for( int i=2; i<argc && fileArgIndex == 1; i++ )
		{
			if( strcmp( argv[i], "-file" ) == 0 )
				fileArgIndex = i;
		}

		const char* inputFile = argv[fileArgIndex+1];
		FILE* f = fopen( inputFile, "rb" );
		if( f )
		{
			fseek( f, 0, SEEK_END );
			int size = ftell( f );
			fseek( f, 0, SEEK_SET );

			char* fileContent = new char[size+1];
			fread( fileContent, 1, size, f );
			fileContent[size] = 0;

			fclose( f );

			inputFileCount = parseInputFiles( inputFiles, rootFolder, fileContent );
			if( inputFileCount < 0 )
			{
				printf( "Too many files in input file list. Max is %d.\n", MAX_INPUT_FILES );
				return 1;
			}
		}
		else
		{
			printf( "Failed to open file: %s\n", inputFile );
			return 1;
		}
	}

	// pack content
	return packContent( inputFiles, inputFileCount, outputName, contentMode );
}

int parseInputFiles( char** inputFiles, char* rootFolder, char* fileContent )
{
	int index = 0;

	int rootLen = 0;
	if( rootFolder )
		rootLen = strlen( rootFolder );

	char* cur = fileContent;
	while( *cur )
	{
		if( index >= MAX_INPUT_FILES )
			return -1;

		while( *cur == '\r' || *cur == '\n' )
			cur++;

		char* start = cur;
		while( *cur != '\r' && *cur != '\n' && *cur )
			cur++;

		int len = cur - start;
		inputFiles[index] = new char[len+rootLen+1];

		int offset = 0;
		if( rootFolder )
		{
			strncpy( inputFiles[index], rootFolder, rootLen );
			offset = rootLen;
		}

		strncpy( inputFiles[index]+offset, start, len );
		inputFiles[index][rootLen+len] = 0;
		index++;
	}

	return index;
}

int packContent( char** inputFiles, int inputFileCount, char* outputName, int contentMode )
{
	if( contentMode == MODE_TEXTURES )
		return packTextures( inputFiles, inputFileCount, outputName );

	printf( "Invalid content mode. Aborting." );
	return 1;
}

int packTextures( char** inputFiles, int inputFileCount, char* outputName )
{
	int result = 0;

	FILE* output = fopen( outputName, "wb" );
	if( output )
	{
		// write number of files
		fwrite( &inputFileCount, sizeof(inputFileCount), 1, output );

		// write name of textures
		for( int i=0; i<inputFileCount; i++ )
		{
			char* inputFile = inputFiles[i];
			int pathLen = strlen( inputFile );

			int slashIndex = -1;
			int dotIndex = -1;
			for( int j=pathLen-1; j>0 && slashIndex < 0; j-- )
			{
				if( inputFile[j] == '.' )
					dotIndex = j;
				else if( inputFile[j] == '\\' || inputFile[j] == '/' )
					slashIndex = j+1;
			}

			if( dotIndex < 0 )
			{
				printf( "No extension found for file: %s\n", inputFile );
				result = 1;
			}
			else 
			{
				if( slashIndex < 0 )
					slashIndex = 0;

				int nameLen = dotIndex - slashIndex;
				if( nameLen >= MAX_TEXTURE_NAME_LEN )
				{
					printf( "Filename is too long: %s\n", inputFile );
				}
				else
				{
					char name[MAX_TEXTURE_NAME_LEN] = {};
					strncpy( name, inputFile + slashIndex, nameLen );

					fwrite( name, 1, MAX_TEXTURE_NAME_LEN, output );
				}
			}
		}

		// write textures
		for( int i=0; i<inputFileCount; i++ )
		{
			char* inputFile = inputFiles[i];

			FILE* input = fopen( inputFile, "rb" );
			if( input )
			{
				char buffer[BUFFER_SIZE] = {};
				int bytesRead = BUFFER_SIZE;
				while( bytesRead >= BUFFER_SIZE )
				{
					bytesRead = fread( buffer, 1, BUFFER_SIZE, input );
					fwrite( buffer, 1, bytesRead, output );
				}

				fclose( input );
			}
			else
			{
				printf( "Failed to open file: %s\n", inputFile );
				result = 1;
			}
		}

		fclose( output );
	}
	else
	{
		printf( "Failed to create output file: %s\n", outputName );
		result = 1;
	}

	return result;
}