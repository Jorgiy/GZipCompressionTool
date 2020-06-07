# GZipCompressionTool
Block-handling based compression tool.
Input file is being processed in mupltiple threads. Each thread operates fixed-sized chunk e.g. in 1 MB.
Each chunk is being handling independent to another ones, synchronization occurs only on file writing.

Usage:
compress/decompress (input file name) (output file name)
