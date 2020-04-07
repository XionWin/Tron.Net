/*
 * wiringSerial.h:
 *	Handle a serial port
 ***********************************************************************
 * This file is part of wiringPi:
 *	https://projects.drogon.net/raspberry-pi/wiringpi/
 *
 *    wiringPi is free software: you can redistribute it and/or modify
 *    it under the terms of the GNU Lesser General Public License as published by
 *    the Free Software Foundation, either version 3 of the License, or
 *    (at your option) any later version.
 *
 *    wiringPi is distributed in the hope that it will be useful,
 *    but WITHOUT ANY WARRANTY; without even the implied warranty of
 *    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *    GNU Lesser General Public License for more details.
 *
 *    You should have received a copy of the GNU Lesser General Public License
 *    along with wiringPi.  If not, see <http://www.gnu.org/licenses/>.
 ***********************************************************************
 */

/* Defines for Serial */
#ifndef SERIAL_H
#define SERIAL_H

#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>
#include <stdarg.h>
#include <string.h>
#include <termios.h>
#include <unistd.h>
#include <fcntl.h>
#include <sys/ioctl.h>
#include <sys/types.h>
#include <sys/stat.h>

#define SERIAL_H_VERSION 10000 /* Version 1.00 */



#ifdef __cplusplus
extern "C" {
#endif

extern int 		serial_open     	(const char *device, const int baudrate);
extern int		serial_setbaudrate	(const int fd, const int baudrate);
extern void		serial_close    	(const int fd);
extern void		serial_flush    	(const int fd);
extern void		serial_writechar  	(const int fd, const unsigned char c);
extern void		serial_write     	(const int fd, const char *s, const int len);
extern int 		serial_readlen		(const int fd);
extern int 		serial_readchar  	(const int fd);
extern int		serial_read 		(const int fd, char *s, const int len);

#ifdef __cplusplus
}
#endif

#endif /* SERIAL_H */
