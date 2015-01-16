#!/usr/bin/python2.7
from __future__ import division
from instrumentino import Instrument
from instrumentino import cfg
from instrumentino.controllers.arduino import SysVarAnalogArduinoUnipolar
from instrumentino.controllers.arduino.pins import AnalogPins
 
'''
*** System constants
'''
# Arduino pin assignments
pinAnalInA = 1
 
'''
*** System components
'''
pins = AnalogPins('', (SysVarAnalogArduinoUnipolar('signal', [0,5], pinAnalInA, None, '', '', 'V', pinInVoltsMax=5, pinInVoltsMin=0),))
 
'''
*** System actions
'''

'''
*** System
'''
class System(Instrument):
    def __init__(self):
        comps = (pins,)
        action = ()
        name = 'Daniels Example System'
        description = 'A container connected, through a valve, to a pressure controller or to the atmosphere'
        version = '1.0'
         
        Instrument.__init__(self, comps, action, version, name, description)
 
'''
*** Run program
'''        
if __name__ == '__main__':
    # run the program
    System()
