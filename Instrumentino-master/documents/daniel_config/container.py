from __future__ import division
from instrumentino import Instrument
from instrumentino import cfg
from instrumentino.action import SysAction, SysActionParamTime, SysActionParamFloat
from instrumentino.controllers.arduino import SysVarDigitalArduino, SysVarAnalogArduinoUnipolar
from instrumentino.controllers.arduino.pins  import DigitalPins, AnalogPins
 
'''
*** System constants
'''
# Arduino pin assignments
pinAnalInA = 1
 
'''
*** System components
'''
analPins = AnalogPins('analogPins', (SysVarAnalogArduinoUnipolar('A',(0,5),
    pinAnalInA, 'analPins', units='volts')))


'''
*** System actions
'''
class SysActionSetPins(SysAction):
    def __init__(self):
        self.seconds = SysActionParamTime()
        self.pinA = SysActionparamFloat(analPins.vars['A'])
        SysAction.__init__(self, 'Set pins', ( self.seconfs, self.pinA ))
        
    def Command(self):
        #Set pins
        analPins.vars['A'].Set(self.pinA.Get())

        #Wait some time
        cfg.Sleep(seld.seconds.Get())

        #Zero pins
        analPins.vars['A'].Set(0)
'''
*** System
'''
class System(Instrument):
    def __init__(self):
        comps = (analPins)
        action = (SysActionSetPins())
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
