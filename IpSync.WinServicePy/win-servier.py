
import win32serviceutil 
import win32service 
import win32event 
import redis

HOST='144.168.57.112'
PORT=6380
DB=0
PASSWORD='P@ssw0rd'

class IpSyncService(win32serviceutil.ServiceFramework): 
    """
    Usage: 'PythonService.py [options] install|update|remove|start [...]|stop|restart [...]|debug [...]'
    Options for 'install' and 'update' commands only:
     --username domain\username : The Username the service is to run under
     --password password : The password for the username
     --startup [manual|auto|disabled|delayed] : How the service starts, default = manual
     --interactive : Allow the service to interact with the desktop.
     --perfmonini file: .ini file to use for registering performance monitor data
     --perfmondll file: .dll file to use when querying the service for
       performance data, default = perfmondata.dll
    Options for 'start' and 'stop' commands only:
     --wait seconds: Wait for the service to actually start or stop.
                     If you specify --wait with the 'stop' option, the service
                     and all dependent services will be stopped, each waiting
                     the specified period.
    """
    _svc_name_ = "IpSyncService"
    _svc_display_name_ = "IpSyncService"
    _svc_description_ = "IpSyncService"

    def __init__(self, args):
        win32serviceutil.ServiceFramework.__init__(self, args)
        # Create an event which we will use to wait on.
        # The "service stop" request will set this event.
        self.hWaitStop = win32event.CreateEvent(None, 0, 0, None)

    def SvcStop(self):
        # Before we do anything, tell the SCM we are starting the stop process.
        self.ReportServiceStatus(win32service.SERVICE_STOP_PENDING)
        # And set my event.
        win32event.SetEvent(self.hWaitStop)

    def SvcDoRun(self):
        # We do nothing other than wait to be stopped!
        # win32event.WaitForSingleObject(self.hWaitStop, win32event.INFINITE)

if __name__=='__main__':
    win32serviceutil.HandleCommandLine(SmallestPythonService)
