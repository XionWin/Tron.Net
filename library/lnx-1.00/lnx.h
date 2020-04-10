/* Defines for framebuffer */
#ifndef PRIORITY_H
#define PRIORITY_H

#include <stdlib.h>
#include <stdio.h>
#include <unistd.h>
#include <pthread.h>
#include <sched.h>


#ifdef __cplusplus
extern "C"
{
#endif

	extern void set_realtime();
	extern void sleep_us(unsigned int usecs);

#ifdef __cplusplus
}
#endif

#endif /* PRIORITY_H */
