using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;


public class BufferManager
{
    int m_numBytes;                 //由缓冲池控制的总字节数
    byte[] m_buffer;                // 由缓冲区管理器维护的底层字节数组 
    Stack<int> m_freeIndexPool;     //   
    int m_currentIndex;
    int m_bufferSize;

    public BufferManager(int totalBytes, int bufferSize)
    {
        m_numBytes = totalBytes;
        m_currentIndex = 0;
        m_bufferSize = bufferSize;
        m_freeIndexPool = new Stack<int>();
    }

    // 分配缓冲池使用的缓冲区空间
    public void InitBuffer()
    {
        //创建一个大的缓冲区并分割它
        //输出给每个SocketAsyncEventArg对象
        m_buffer = new byte[m_numBytes];
    }

    //将缓冲池中的缓冲区分配给
    //指定SocketAsyncEventArgs对象 
    //  
    // <returns>如果缓冲区已成功设置，则为true，否则为false</returns>  
    public bool SetBuffer(SocketAsyncEventArgs args)
    {

        if (m_freeIndexPool.Count > 0)
        {
            args.SetBuffer(m_buffer, m_freeIndexPool.Pop(), m_bufferSize);
        }
        else
        {
            if ((m_numBytes - m_bufferSize) < m_currentIndex)
            {
                return false;
            }
            args.SetBuffer(m_buffer, m_currentIndex, m_bufferSize);
            m_currentIndex += m_bufferSize;
        }
        return true;
    }

    //从SocketAsyncEventArg对象中移除缓冲区。
    //这将缓冲区释放回缓冲池
    public void FreeBuffer(SocketAsyncEventArgs args)
    {
        m_freeIndexPool.Push(args.Offset);
        args.SetBuffer(null, 0, 0);
    }
}

