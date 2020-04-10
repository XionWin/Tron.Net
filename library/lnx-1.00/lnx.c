#include "lnx.h"

void set_realtime()
{
    pid_t pid = getpid();
    struct sched_param param;
    param.sched_priority = sched_get_priority_max(SCHED_FIFO);
    sched_setscheduler(pid, SCHED_FIFO, &param);
    pthread_setschedparam(pthread_self(), SCHED_FIFO, &param);
}

void sleep_us(unsigned int usecs)
{
    usleep(usecs);
}
