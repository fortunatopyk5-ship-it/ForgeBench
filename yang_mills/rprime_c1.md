# YM-RPRIME-C1-1 — source-instantiated regular R' cluster/log C^1 theorem (Iteration 8)

Scope:
Only the regular/second-group R' cluster-log sector of Balaban CMP122-II. No full history map, no K_0, no STEP_0, no physical source differentiation.

## 1. Source object and regular projection

Fix one k-th R-operation and the source domains Y_1,...,Y_m that are held fixed in the exponentiation step.

CMP122-II Eq. (1.90) gives a polymer expansion

Xi(F)
 =
1+
sum_{r>=1}
sum'_{ {X'_1,...,X'_r} }
product_{p=1}^r F(X'_p),

where X'_p are connected localization components and the primed family is the hard-core family produced by the disjoint component decomposition.

Eq. (1.91) defines each pre-exponentiation activity F(X') by resumming all raw expressions that determine exactly the connected localization domain X'.

Let

Ycal = union_{i=1}^m Y_i.

For a source component eta=X' disjoint from Ycal, primary Eq. (1.97) gives

||F_eta^src||
 <=
 exp[-p_0(g_k)]
 exp[-kappa_in d_k(eta)],                                 (R1)

where

kappa_in=(1+beta)kappa.

Reason:
Eq. (1.97) gives the relative metric d_{k,Ycal}; primary Eq. (1.67) defines it by a shortest tree intersecting components of eta\Ycal. If eta cap Ycal is empty, eta\Ycal=eta and therefore

d_{k,Ycal}(eta)=d_k(eta).

The source later splits the logarithmic output R'^(k)(X) into:
- first group: boundary B' terms;
- second group: regular R terms.

For the second group primary CMP122-II states that c_1=exp[-p_0(g_k)] and the ordinary d_k(X) replaces the relative metric, yielding Eq. (1.100).

We call this second group the regular R' sector.

## 2. Common coefficient algebra for coefficient-C^1

No physical field/source derivative is taken in this theorem.

For each finite-volume/source-history step, let Xscr_k be the set of admissible field/source configurations on which the local source activities in (1.90) are evaluated.

Let

A_k = B_b(Xscr_k; C)

with the sup norm and pointwise multiplication.

Every source local cylinder activity F_eta is pulled back to Xscr_k by restriction of the global configuration to eta. Thus all activities may be regarded as elements of the same commutative unital Banach algebra A_k, and

||fg||_{A_k}<=||f||_{A_k}||g||_{A_k}.

This is only a coefficient algebra. It makes no claim about a uniform complex collar in the physical U/J variables.

## 3. Exact regular input anchored norm

Let P_k^reg be the connected localization domains eta relevant to the regular lane, in particular eta cap Ycal=empty when using (R1).

For mu_in>=0 define

||F||_in
 :=
 sup_{B in pi_k}
 sum_{eta in P_k^reg: eta contains B}
 e^{mu_in d_k(eta)} ||F_eta||_{A_k}.                      (R2)

This defines the input Banach space B_in,k(mu_in).

Choose a fixed lambda_in in the CMP109 geometric-summability regime and set

mu_in
 :=
 kappa_in-lambda_in
 =
 (1+beta)kappa-lambda_in
 >0.                                                       (R3)

By CMP109 Eq. (0.26),

C_geo(lambda_in)
 :=
 sup_{k,B}
 sum_{eta contains B}
 e^{-lambda_in d_k(eta)}
 <infinity.                                                (R4)

Combining (R1)-(R4),

||F^src||_in
 <=
 r_src(g_k)
 :=
 C_geo(lambda_in)e^{-p_0(g_k)}.                           (R5)

This is the requested source pointwise -> anchored norm implication.

## 4. Primary small-coupling input

CMP119 introduction directly defines the large-field profile in the form

p_0(g)
 =
 A_0 (log g^{-2})^{p_*},                                  (R6)

with positive integer p_* (and the positive large-field constant A_0 in the construction).

Hence, on 0<g<=gamma<1,

p_0(g)>=p_0(gamma),

and

p_0(gamma)->infinity
as gamma->0.                                               (R7)

Therefore

r_src(g)
<=
r_src(gamma)
 :=
C_geo(lambda_in)e^{-p_0(gamma)}
 ->0                                                       (R8)

as gamma->0.

This is the only small-coupling fact needed below to place the actual source activity strictly inside an explicit activity ball.

The detailed CMP122-II derivation of (1.97) itself uses its own source restrictions (small alpha_LF, large kappa/K, and g_k sufficiently small); those restrictions remain part of the source hypotheses and are not replaced by our KP condition.

## 5. Tree stitching for connected hard-core clusters

For a connected localization domain X, d_k(X) is the shortest-tree length in the rescaled pi_k-cube geometry.

Let eta_1,...,eta_n be polymers whose incompatibility/overlap graph is connected and let

gamma=union_i eta_i.

In d=4,

d_k(gamma)
 <=
 sum_i d_k(eta_i)
 + c_box(n-1),                                             (R9)

where one may take

c_box=sqrt(4)=2.

Proof:
Choose a shortest tree inside each eta_i. Choose a spanning tree of the overlap graph. For each spanning-tree edge, eta_i and eta_j share a unit pi_k cube. Their two internal trees each meet that cube. Join the two meeting points by a segment inside the common unit cube; this costs at most its Euclidean diameter 2. The union is a connected tree in gamma meeting every cube.

This is the only new geometric ingredient needed to preserve an output tree weight through the cluster logarithm.

## 6. Explicit tree-weighted KP ball

Fix constants

a>0,
theta>0

such that

16a+theta <= mu_in.                                       (R10)

The factor 16 is the already proved d=4 coercivity

|eta|<=16(d_k(eta)+1).

Define the generalized KP weights

a_KP(eta)=a|eta|,

g_theta(eta)=theta d_k(eta)+2theta.                        (R11)

For any F in B_in,k(mu_in),

sup_B
sum_{eta contains B}
||F_eta||
exp[a_KP(eta)+g_theta(eta)]

<=
exp(16a+2theta)||F||_in.                                  (R12)

Indeed,
a|eta|+theta d(eta)+2theta
<=
(16a+theta)d(eta)+16a+2theta
<=
mu_in d(eta)+16a+2theta.

Set

q(rho)
 :=
 e^{16a+2theta} rho,                                      (R13)

and define the EXPLICIT sufficient convergence radius

r_*
 :=
 a e^{-16a-2theta}.                                       (R14)

If

||F||_in <= rho < r_*,

then q(rho)<a.

For every polymer eta_0, hard-core incompatibility is overlap, hence

sum_{eta incompatible with eta_0}
||F_eta|| e^{a_KP(eta)+g_theta(eta)}

<=
|eta_0| q(rho)
<
a|eta_0|
=
a_KP(eta_0).                                              (R15)

Thus the generalized Kotecky-Preiss criterion holds uniformly in finite volume, cutoff, and scale.

No hidden "sufficiently small" activity radius remains: (R14) is a concrete sufficient radius.

## 7. Canonical cluster logarithm and exact-union support

Define the canonical connected logarithm of the source polymer gas (1.90) by

H_R'(F)_gamma
 :=
 sum_{n>=1} 1/n!
 sum_{eta_1,...,eta_n:
      incompatibility graph connected,
      union_i eta_i=gamma}
 phi^T(eta_1,...,eta_n)
 product_i F_{eta_i}.                                     (R16)

Here phi^T is the standard Ursell coefficient of the hard-core overlap graph.

Eq. (1.98) identifies the source exponentiation with this connected logarithm:

Xi(F)=exp[sum_gamma H_R'(F)_gamma].

The exact-union grouping in (R16) removes the need to rely on the suspicious printed reference "(7.13) [I]" for the support rule.

### Exact support theorem

Let

M_{gamma eta}(F)
 :=
D_eta H_R'(F)_gamma.

Then

M_{gamma eta}(F)=0
unless eta subset gamma.                                  (R17)

Proof:
Every monomial contributing to H_R'(F)_gamma has union exactly gamma. Differentiation with respect to F_eta can be nonzero only if eta is one of its constituent polymers, hence eta subset gamma.

Fix once and for all a total order on pi_k cubes and define

s(gamma,eta)
 :=
 the least cube of eta.                                   (R18)

For every nonzero Jacobian entry,
s(gamma,eta) belongs to eta subset gamma.

Therefore the source/cluster collar for this coefficient map is

r_col=0.                                                   (R19)

### Regular-sector decoupling

If an output gamma belongs to the source second/regular group and is disjoint from the source boundary/large-field set, every constituent eta in (R16) is contained in gamma and is therefore also disjoint from that set. Hence boundary-lane input activities cannot contribute to a regular output coefficient.

Thus the source regular R' coefficient is exactly the regular projection of the logarithm of the regular input activity family.

## 8. Uniform output bound and holomorphy

Let

B_out,k(theta)

be the anchored output activity space with norm

||K||_theta
 =
sup_B sum_{gamma contains B}
e^{theta d_k(gamma)}||K_gamma||_{A_k}.                    (R20)

The generalized KP rooted-cluster estimate together with the tree stitching (R9) yields

||H_R'(F)||_theta
<=
q(rho)                                                     (R21)

for every ||F||_in<=rho<r_*.

Sketch of the anchored estimate:
for an overlap-connected cluster with union gamma, (R9) gives

e^{theta d(gamma)}
<=
product_i e^{g_theta(eta_i)}.

If gamma contains B, mark one constituent eta_1 containing B. The 1/n! cluster coefficient and the sum over the possible mark are bounded by the standard rooted KP estimate. Summing the marked root gives exactly the left side of (R12), hence at most q(rho).

The cluster series is absolutely normally convergent in B_out,k(theta) on every closed input ball of radius rho<r_*.

Finite-volume partial sums are Banach-valued holomorphic polynomials in F. Uniform normal convergence therefore implies, by the Banach-valued Weierstrass theorem,

H_R':
B_{r_*}(B_in,k(mu_in))
 ->
B_out,k(theta)                                             (R22)

is holomorphic.

The estimate and holomorphy are uniform in:
- total volume;
- UV cutoff;
- RG depth k.

The spaces are scale-indexed, but the constants and norm structure are uniform.

## 9. Strict source radius

Choose the Balaban small-coupling threshold gamma_R'>0 so small that

r
 :=
 C_geo(lambda_in)e^{-p_0(gamma_R')}
 <
 r_*.                                                      (R23)

Existence follows from (R7)-(R8).

This is compatible with CMP122-II Theorem 1, whose hypothesis already permits the effective-coupling interval ]0,gamma] with gamma sufficiently small; we simply shrink that fixed threshold further if necessary.

For every source scale satisfying

0<g_k<=gamma_R',

the source regular input obeys

||F^src||_in<=r.                                          (R24)

Define

rho
 :=
 (r+r_*)/2,                                               (R25)

delta_hist
 :=
 rho-r
 =
 (r_*-r)/2
 >0.                                                       (R26)

Thus

r<rho<r_*

with all three radii independent of volume, cutoff, and RG depth.

No numerical value is invented.

## 10. Cauchy component derivative

Fix F with ||F||_in<=r.

For a unit vector h in the eta-coordinate algebra A_k, the coordinate injection satisfies

||I_eta h||_in
=
e^{mu_in d_k(eta)}.

Therefore

F+z I_eta h

stays in the rho-ball whenever

|z|
<
(rho-r)e^{-mu_in d_k(eta)}.

By Banach-valued Cauchy applied to the gamma-coordinate and (R21),

||M_{gamma eta}(F)||
<=
[q(rho)/(rho-r)]
e^{mu_in d_k(eta)}
e^{-theta d_k(gamma)}.                                   (R27)

This is a full-Banach-space Cauchy estimate, not a coordinatewise analyticity assumption.

## 11. Source-geometry Schur sum

Define the anchor distance

dist_k(a,b)

as the Euclidean distance between the two rescaled unit pi_k cubes a,b.

Because nonzero M_{gamma eta} implies
a=s(gamma,eta) in eta subset gamma
and b in gamma,

the source shortest-tree metric obeys

d_k(gamma)>=dist_k(a,b).                                  (R28)

Fix nu>0 and mu_out>=0 such that

lambda_res
 :=
 theta-mu_out-nu                                          (R29)

lies in the CMP109 geometric-summability regime.

Then (R27), (R17), and (R28) give

sup_{eta contains a}
[
 e^{-mu_in d_k(eta)}
 sum_{gamma contains b:
      s(gamma,eta)=a}
 e^{mu_out d_k(gamma)}
 ||M_{gamma eta}(F)||
]

<=
[q(rho)/(rho-r)]
C_geo(lambda_res)
e^{-nu dist_k(a,b)}.                                      (R30)

Define

C_lat(nu)
 :=
sup_b sum_a e^{-nu dist_k(a,b)}.                          (R31)

The rescaled block lattice is Z^4 up to finite-volume periodic identification, so

C_lat(nu)<infinity

with a bound independent of scale and volume.

Applying YM-HISTORY-JACOBIAN-QUASILOCAL-1,

||D H_R'(F)||_{B_in,k(mu_in)->B_out,k(mu_out)}
<=
C_R',                                                      (R32)

where the explicit symbolic constant is

C_R'
 :=
[q(rho)/(rho-r)]
C_geo(lambda_res)
C_lat(nu).                                                 (R33)

All factors are defined and cutoff/volume/depth independent.

## 12. YM-RPRIME-C1-1

Theorem:
Fix Balaban source parameters beta,kappa and choose:

1. lambda_in in the CMP109 summability regime with
   mu_in=(1+beta)kappa-lambda_in>0;

2. a,theta>0 satisfying
   16a+theta<=mu_in;

3. r_*=a exp[-16a-2theta];

4. gamma_R'>0 small enough that
   r=C_geo(lambda_in)e^{-p_0(gamma_R')}<r_*;

5. rho=(r+r_*)/2;

6. nu>0 and mu_out>=0 such that
   lambda_res=theta-mu_out-nu
   lies in the CMP109 summability regime.

Then for every RG scale k with g_k<=gamma_R', the regular second-group R' cluster/log map associated with CMP122-II (1.90)-(1.100),

H_R',k^reg:
B_r(B_in,k(mu_in))
 ->
B_out,k(mu_out),

is holomorphic (in fact it is the restriction of a holomorphic map on the r_*-ball), hence C^1, and

sup_{||F||_in<=r}
||D H_R',k^reg(F)||
<=
C_R'

with C_R' given by (R33), independent of volume, cutoff, and k.

Moreover its Jacobian is quasi-local in the weighted Schur sense (R30), and

D_eta H_R',gamma=0 unless eta subset gamma.

Status:
PROVED, using:
- DIRECT PRIMARY CMP119 p_0 definition;
- DIRECT PRIMARY CMP122-II (1.90)-(1.100);
- DIRECT PRIMARY CMP109 anchored tree summability;
- generalized KP/rooted-cluster theorem;
- the proved tree coercivity and the elementary tree-stitching lemma above.

This theorem is ONLY for the regular R' cluster/log coefficient sector.

It does NOT prove:
- the boundary B' lane;
- the preceding construction map from earlier E/R/B/T data to F;
- the full compressed history map;
- a physical source derivative.

## 13. Decay-reserve wording

The source exponents before and after the Balaban exponentiation are

(1+beta)kappa
and
(1+beta/2)kappa.

Their difference

(beta/2)kappa

is recorded only as the

SOURCE-VISIBLE DECAY RESERVE.

The actual Banach operator loss in YM-RPRIME-C1-1 is

mu_in-mu_out,

which also spends whatever geometric summability/off-diagonal reserves were chosen in lambda_in, theta, lambda_res, and nu.

These two quantities are not identified.

## 14. Source insertion map status

Introduce a physical source space J_A and an insertion map

jmath_A:J_A->B_in,k(mu_in).

No boundedness of jmath_A is proved here.

Therefore no statement of the form

J_A in B_in

is made automatically.

The present theorem is a coefficient-space C^1 result only.

## 15. Final Iteration-8 verdict

The requested source-instantiated bounded C^1 theorem for the REGULAR R' cluster/log sector is closed.

The next blocker, outside this run's allowed scope, is not R' exponentiation. It is upstream:
the bounded C^1/source-local map that constructs the pre-exponentiation activity F from the earlier localized T/E/R/B history data (and, separately, the boundary B' lane).

No K_0.
No STEP_0.
