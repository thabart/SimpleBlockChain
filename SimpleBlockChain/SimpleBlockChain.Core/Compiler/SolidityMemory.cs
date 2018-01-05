using System;
using System.Collections.Generic;

namespace SimpleBlockChain.Core.Compiler
{
    public class SolidityMemory
    {
        private List<byte[]> _chunks = new List<byte[]>();
        private int _softSize;
        public static byte[] EMPTY_BYTE_ARRAY = new byte[0];
        private static int CHUNK_SIZE = 1024;
        private static int WORD_SIZE = 32;

        public byte[] Read(int address, int size)
        {
            if (size <= 0)
            {
                return EMPTY_BYTE_ARRAY;
            }

            Extend(address, size);
            byte[] data = new byte[size];
            int chunkIndex = address / CHUNK_SIZE;
            int chunkOffset = address % CHUNK_SIZE;
            int toGrab = data.Length;
            int start = 0;
            while (toGrab > 0)
            {
                int copied = GrabMax(chunkIndex, chunkOffset, toGrab, data, start);
                ++chunkIndex;
                chunkOffset = 0;
                toGrab -= copied;
                start += copied;
            }

            return data;
        }

        public void Write(int address, byte[] data, int dataSize, bool limited)
        {
            if (data.Length < dataSize) { dataSize = data.Length; }
            if (!limited) { Extend(address, dataSize); }
            int chunkIndex = address / CHUNK_SIZE;
            int chunkOffset = address % CHUNK_SIZE;
            int toCapture = 0;
            if (limited) { toCapture = (address + dataSize > _softSize) ? _softSize - address : dataSize; }
            else { toCapture = dataSize; }
            int start = 0;
            while (toCapture > 0)
            {
                int captured = CaptureMax(chunkIndex, chunkOffset, toCapture, data, start);
                ++chunkIndex;
                chunkOffset = 0;
                toCapture -= captured;
                start += captured;
            }
        }

        public void ExtendAndWrite(int address, int allocSize, byte[] data)
        {
            Extend(address, allocSize);
            Write(address, data, data.Length, false);
        }

        public void Extend(int address, int size)
        {
            if (size <= 0) { return; }
            var newSize = address + size;
            int toAllocate = newSize - GetInternalSize();
            if (toAllocate > 0)
            {
                AddChunks((int)Math.Ceiling((double)toAllocate / CHUNK_SIZE));
            }

            toAllocate = newSize - _softSize;
            if (toAllocate > 0)
            {
                toAllocate = (int)Math.Ceiling((double)toAllocate / WORD_SIZE) * WORD_SIZE;
                _softSize += toAllocate;
            }
        }

        public int GetInternalSize()
        {
            return _chunks.Count * CHUNK_SIZE;
        }

        public List<byte[]> GetChunks()
        {
            return _chunks;
        }

        public int GetSize()
        {
            return _softSize;
        }

        private void AddChunks(int num)
        {
            for (int i = 0; i < num; ++i)
            {
                _chunks.Add(new byte[CHUNK_SIZE]);
            }
        }

        private int CaptureMax(int chunkIndex, int chunkOffset, int size, byte[] src, int srcPos)
        {
            byte[] chunk = _chunks[chunkIndex];
            int toCapture = Math.Min(size, chunk.Length - chunkOffset);
            Array.Copy(src, srcPos, chunk, chunkOffset, toCapture);
            return toCapture;
        }

        private int GrabMax(int chunkIndex, int chunkOffset, int size, byte[] dest, int destPos)
        {
            byte[] chunk = _chunks[chunkIndex];
            int toGrab = Math.Min(size, chunk.Length - chunkOffset);
            Array.Copy(chunk, chunkOffset, dest, destPos, toGrab);
            return toGrab;
        }
    }
}
