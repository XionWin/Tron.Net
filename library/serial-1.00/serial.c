/*
 * wiringSerial.c:
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

#include "serial.h"

/*
 * serialOpen:
 *	Open and initialise the serial port, setting all the right
 *	port parameters - or as many as are required - hopefully!
 *********************************************************************************
 */

int serial_open (const char *device, const int baudrate)
{
  int status, fd;

  if ((fd = open (device, O_RDWR | O_NOCTTY | O_NDELAY | O_NONBLOCK)) == -1)
    return -1;

  fcntl (fd, F_SETFL, O_RDWR);

  if (!serial_setbaudrate(fd, baudrate))
    return -2;
    
  ioctl (fd, TIOCMGET, &status);

  status |= TIOCM_DTR;
  status |= TIOCM_RTS;

  ioctl (fd, TIOCMSET, &status);

  usleep (10000);	// 10mS

  return fd;
}

int serial_setbaudrate	(const int fd, const int baudrate)
{
  struct termios options;
  speed_t myBaud;
  int result = 0;

  switch (baudrate)
  {
    case      50:	myBaud =      B50; break;
    case      75:	myBaud =      B75; break;
    case     110:	myBaud =     B110; break;
    case     134:	myBaud =     B134; break;
    case     150:	myBaud =     B150; break;
    case     200:	myBaud =     B200; break;
    case     300:	myBaud =     B300; break;
    case     600:	myBaud =     B600; break;
    case    1200:	myBaud =    B1200; break;
    case    1800:	myBaud =    B1800; break;
    case    2400:	myBaud =    B2400; break;
    case    4800:	myBaud =    B4800; break;
    case    9600:	myBaud =    B9600; break;
    case   19200:	myBaud =   B19200; break;
    case   38400:	myBaud =   B38400; break;
    case   57600:	myBaud =   B57600; break;
    case  115200:	myBaud =  B115200; break;
    case  230400:	myBaud =  B230400; break;
    case  460800:	myBaud =  B460800; break;
    case  500000:	myBaud =  B500000; break;
    case  576000:	myBaud =  B576000; break;
    case  921600:	myBaud =  B921600; break;
    case 1000000:	myBaud = B1000000; break;
    case 1152000:	myBaud = B1152000; break;
    case 1500000:	myBaud = B1500000; break;
    case 2000000:	myBaud = B2000000; break;
    case 2500000:	myBaud = B2500000; break;
    case 3000000:	myBaud = B3000000; break;
    case 3500000:	myBaud = B3500000; break;
    case 4000000:	myBaud = B4000000; break;

    default:
      return 0;
  }

  // Get and modify current options:

  tcgetattr (fd, &options);

  cfmakeraw   (&options);
  result |= cfsetispeed (&options, myBaud);
  result |= cfsetospeed (&options, myBaud);

  options.c_cflag |= (CLOCAL | CREAD);
  options.c_cflag &= ~PARENB;
  options.c_cflag &= ~CSTOPB;
  options.c_cflag &= ~CSIZE;
  options.c_cflag |= CS8;
  options.c_lflag &= ~(ICANON | ECHO | ECHOE | ISIG);
  options.c_oflag &= ~OPOST;

  options.c_cc [VMIN]  = 0;
  options.c_cc [VTIME] = 0;	// wait for 0 milliseconds

  tcsetattr (fd, TCSANOW, &options);


  usleep (10000);	// 10mS

  return result == 0;
}


/*
 * serialClose:
 *	Release the serial port
 *********************************************************************************
 */

void serial_close (const int fd)
{
  close (fd);
}

/*
 * serialFlush:
 *	Flush the serial buffers (both tx & rx)
 *********************************************************************************
 */

void serial_flush (const int fd)
{
  tcflush (fd, TCIOFLUSH);
}



/*
 * serialPutchar:
 *	Send a single character to the serial port
 *********************************************************************************
 */

void serial_writechar (const int fd, const unsigned char c)
{
  write (fd, &c, 1);
}


/*
 * serialPuts:
 *	Send a string to the serial port
 *********************************************************************************
 */

void serial_write (const int fd, const char *s, const int len)
{
  write (fd, s, len);
}

/*
 * serialDataAvail:
 *	Return the number of bytes of data avalable to be read in the serial port
 *********************************************************************************
 */

int serial_readlen (const int fd)
{
  int result;

  if (ioctl (fd, FIONREAD, &result) == -1)
    return -1;

  return result;
}


/*
 * serialGetchar:
 *	Get a single character from the serial device.
 *	Note: Zero is a valid character and this function will time-out after
 *	10 seconds.
 *********************************************************************************
 */

int serial_readchar (const int fd)
{
  uint8_t x;

  if (read (fd, &x, 1) != 1)
    return -1;

  return ((int)x) & 0xFF;
}

/*
 * serialPuts:
 *	Receive in a string from the serial port
 *********************************************************************************
 */

int serial_read (const int fd, char *s, const int len)
{
  return read (fd, s, len);
}