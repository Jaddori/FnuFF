#pragma once

inline bool isWhitespace( char c )
{
	return ( c == ' ' || c == '\t' );
}

inline bool isNewline( char c )
{
	return ( c == '\r' || c == '\n' );
}

inline int charToInt( char c )
{
	return ( c - '0' );
}