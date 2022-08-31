using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

internal class DisposeAndFinalize
{
    public class MyResource : IDisposable
    {
        // Pointer to an external unmanaged resource.
        private IntPtr handle;
        // Other managed resource this class uses.
        private Component component = new Component();
        // Track whether Dispose has been called.
        private bool disposed = false;

        // The class constructor.
        public MyResource(IntPtr handle)
        {
            this.handle = handle;
            Debug.WriteLine("MyResource created");
        }

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Debug.WriteLine("MyResource.Dispose()");
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
            Debug.WriteLine($"MyResource.Dispose(bool disposing: {disposing})");
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    component.Dispose();
                    Debug.WriteLine("MyResource: Managed resources disposed");
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                CloseHandle(handle);
                handle = IntPtr.Zero;

                Debug.WriteLine("MyResource: Unmanaged resources disposed");

                // Note disposing has been done.
                disposed = true;

            }
        }

        // Use interop to call the method necessary
        // to clean up the unmanaged resource.
        [System.Runtime.InteropServices.DllImport("Kernel32")]
        private extern static Boolean CloseHandle(IntPtr handle);

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~MyResource()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Debug.WriteLine("MyResource.~MyResource()");
            Dispose(false);
        }
    }


    [System.Runtime.InteropServices.DllImport("Kernel32")]
    static extern IntPtr CreateWaitableTimer(IntPtr lpTimerAttributes, bool bManualReset, string lpTimerName);

    static void Main(string[] args)
    {
        N();
        L();
    }

    static void N()
    {
        Class1 c = new Class1();
    }

    static void L()
    {
        MyResource r = new(CreateWaitableTimer(IntPtr.Zero, true, "WaitableTimer"));

        ////////////////////////////////////////////////////////////////////////////////
        r.Dispose(); // Comment this line to see disposing via destructor (finalization)
        ////////////////////////////////////////////////////////////////////////////////
    }

    class Class1
    {
        public Class1()
        {
            Debug.WriteLine("Class1 instance created");
        }

        ~Class1()
        {
            Debug.WriteLine("Class1 instance destroyed");
        }
    }
}