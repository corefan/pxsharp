using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PxSharp {
    partial struct PxPhysics {
        public PxShape CreateShape (PxGeometry geometry, PxMaterial material) {
            return CreateShape(geometry, material, false);
        }

        public PxShape CreateShape (PxGeometry geometry, PxMaterial material, bool isExclusive) {
            return CreateShape(geometry, material, isExclusive, PxShapeFlag.eSCENE_QUERY_SHAPE | PxShapeFlag.eSIMULATION_SHAPE | PxShapeFlag.eVISUALIZATION);
        }
    }
}
