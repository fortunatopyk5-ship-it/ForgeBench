# YM-RPRIME-C1-1 — audited native Balaban regular R' bounded C^1 theorem (Iteration 9)

Scope:
Only the regular/second-group CMP122-II R' cluster/log coefficient sector. No upstream Eq. (1.91) analysis, no boundary B' theorem, no full history map, no K_0, no STEP_0, no physical source differentiation.

## Iteration 9 audit correction — resolved

The temporary Iteration-9 downgrade has been resolved by direct-primary CMP116.

CMP116 (2.11) supplies the exact polymer compatibility convention used by Balaban's cluster expansion:

zeta(eta,eta')=0

iff eta cap eta' contains either a pi_k cube or a full wall of a pi_k cube; otherwise zeta=1.

Thus the native incompatibility support is the closed one-step wall neighborhood of the cube carrier. In d=4,

C_inc=2d+1=9.

For a native incompatibility edge, two constituent unit cubes either coincide or share a wall. The union of two wall-adjacent 4D unit cubes has Euclidean diameter sqrt(7), so

c_stitch=sqrt(7).

CMP122-II says native R'^(k)(X) is given by series (7.13) [I] with proper notational changes. The legacy source numbering is resolved by the published continuation CMP116:
- CMP122-II calls the first Mayer step (7.1) [I];
- published CMP116 Sect. 2 calls its first Mayer step (2.1);
- published CMP116 (2.13) is the localized connected Ursell coefficient grouped by exact union.

The published CMP109 version ends before this cluster construction and explicitly defers it to the next paper. Therefore the legacy (7.13) [I] citation is source-crosswalked to published CMP116 (2.13).

Hence, after the notational replacement H -> F and E -> R',

R'^(k)_Bal(X)=H_X^can(F)

coordinatewise.

This file now records the NATIVE theorem with corrected constants.

## 1. Source object and regular projection

Fix one k-th R-operation and the source domains Y_1,...,Y_m that are held fixed in the exponentiation step.

CMP122-II Eq. (1.90) gives a polymer expansion

Xi(F)
 =
1+
sum_{r>=1}
sum'_{ {X'_1,...,X'_r} }
product_{p=1}^r F(X'_p),

where X'_p are connected localization components. Direct primary source verifies that members of each actual primed family arise as distinct components of the special decomposition and therefore have pairwise disjoint pi_k-cube carriers. The exact converse characterization of the prime is NOT source-extracted.

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

The detailed CMP122-II derivation of (1.97) itself uses its own source restrictions; these are logically PRIOR to our activity-ball argument and are not replaced by it.

Direct-primary conditions visible in the derivation around (1.92)-(1.97) include sufficient inequalities of the source shapes

alpha_LF^(1/3)
<=
exp[-(1+beta)kappa*2d],

O(1) alpha_LF^(1/3)
<=
(6*2^d)^(-1) beta kappa,

and, after taking g_k sufficiently small,

exp[-p_0(g_k)/2]
<=
(6*2^d)^(-1) beta kappa.

The source also imposes its other stated large-K / localization restrictions needed for (1.97).

These conditions establish the concrete RG-generated F and its pointwise decay. They are NOT the radius of the coefficient-space cluster map.

Our separate generalized-KP condition in Section 6 establishes holomorphy for every activity family in an explicit anchored Banach ball.

## 5. Tree stitching for connected hard-core clusters

For a connected localization domain X, d_k(X) is the shortest-tree length in the rescaled pi_k-cube geometry.

Let eta_1,...,eta_n be polymers whose NATIVE incompatibility graph is connected and let

gamma=union_i eta_i.

CMP116 (2.11) says an incompatibility edge means that two polymers share either a pi_k cube or a full wall of a pi_k cube.

In d=4,

d_k(gamma)
 <=
 sum_i d_k(eta_i)
 + sqrt(7)(n-1).                                          (R9)

Proof:
Choose a shortest tree inside each eta_i and a spanning tree of the incompatibility graph. For every spanning-tree edge choose either a common unit cube or two unit cubes sharing a wall. Each internal tree meets its chosen cube. In the worst case the two cubes form a 4D box of side lengths 2,1,1,1, whose Euclidean diameter is sqrt(7). Joining the two meeting points inside this box costs at most sqrt(7). Repeat over the n-1 spanning-tree edges.

Thus

c_stitch=sqrt(7)

is a rigorous native uniform choice.

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

g_theta(eta)=theta d_k(eta)+sqrt(7)theta.                        (R11)

For any F in B_in,k(mu_in),

sup_B
sum_{eta contains B}
||F_eta||
exp[a_KP(eta)+g_theta(eta)]

<=
exp(16a+sqrt(7)theta)||F||_in.                                  (R12)

Indeed,
a|eta|+theta d(eta)+sqrt(7)theta
<=
(16a+theta)d(eta)+16a+sqrt(7)theta
<=
mu_in d(eta)+16a+sqrt(7)theta.

Set

q(rho)
 :=
 e^{16a+sqrt(7)theta} rho,                                      (R13)

and define the EXPLICIT sufficient convergence radius

r_*
 :=
 (a/9) e^{-16a-sqrt(7)theta}.                                       (R14)

If

||F||_in <= rho < r_*,

then

9q(rho)<a.

Define Car(eta) as the pi_k-cube carrier and

Supp_*(eta)=N_wall[Car(eta)].

In d=4,

#Supp_*(eta)<=9|eta|.

Therefore for every native polymer eta_0,

sum_{eta incompatible with eta_0}
||F_eta|| e^{a_KP(eta)+g_theta(eta)}

<=
9|eta_0| q(rho).

The native KP condition is

9q(rho)<a,

which is guaranteed by

rho<r_*,

r_*=(a/9)e^{-16a-sqrt(7)theta}.                           (R15)

Thus the generalized Kotecky-Preiss criterion holds uniformly in finite volume, cutoff, and scale for the NATIVE cube-or-wall hard-core system.

No hidden "sufficiently small" activity radius is used: the native sufficient radius is explicitly

r_*=(a/9)e^{-16a-sqrt(7)theta}.

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

Here phi^T is the standard Ursell coefficient built from the native cube-or-wall incompatibility function zeta of CMP116 (2.11).

Eq. (1.98) identifies the source expression as an exponentiated convergent connected cluster logarithm:

Xi(F)=exp[sum_gamma H_R'(F)_gamma].

CMP122-II then says each native R'^(k)(X) is given by the legacy series (7.13) [I] with proper notational changes. The source-crosswalk established in Iteration 9 identifies that legacy formula with published CMP116 (2.13), whose defining constraint is exact union of the constituent polymers.

Therefore (R16) is not merely an auxiliary localization choice:

R'^(k)_Bal(gamma)=H_R'(F)_gamma

coordinatewise.

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

Published CMP116 (2.13) defines the localized connected coefficient by the exact-union constraint. The CMP122-II legacy citation (7.13) [I] is the same cluster formula under the source's earlier numbering, as certified by the (7.1) [I] Mayer-step <-> published CMP116 (2.1) crosswalk.

Therefore, with the stated notational changes,

R'^(k)_Bal(gamma)=H_R'(F)_gamma.

Thus the exact-union derivative theorem applies directly to Balaban's native R' coordinates.

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

The cluster series is absolutely normally convergent in B_out,k(theta) on every closed input ball of radius rho<r_*. This statement is made directly on the countable anchored polymer Banach space; finite tori are restrictions of the same majorant.

Partial sums by cluster size/support are Banach-valued holomorphic polynomials in F. Normal convergence in the B_out norm therefore implies, by the Banach-valued Weierstrass theorem,

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

## 11. Bounded C^1 closes before the Schur step

The bounded C^1 theorem does NOT require a second CMP109 summability reserve.

By Section 8, for every rho<r_*,

H_R':
B_rho(B_in,k(mu_in))
->
B_out,k(theta)

is holomorphic and

sup_{||F||_in<=rho}
||H_R'(F)||_theta
<=q(rho).

Hence Banach-valued Cauchy directly gives, on the smaller radius-r ball,

boxed:
sup_{||F||_in<=r}
||D H_R'(F)||_{B_in,k(mu_in)->B_out,k(theta)}
<=
q(rho)/(rho-r).                                           (R30)

This constant is independent of volume, cutoff and RG depth.

Thus source-uniform bounded C^1 of the regular R' coefficient map is already closed once the explicit strict radius in Section 9 has been obtained.

No numerical lower bound on the source localization exponent kappa is needed beyond the explicit input-budget condition

mu_in=(1+beta)kappa-lambda_in>0

for some lambda_in in the already verified CMP109 summability regime.

This condition is kept symbolic; no qualitative "kappa sufficiently large" statement is converted into a fake number.

## 12. Quasi-local Jacobian: additional decay-budget condition

For the stronger off-diagonal Jacobian estimate, use the component Cauchy bound

||M_{gamma eta}(F)||
<=
[q(rho)/(rho-r)]
e^{mu_in d_k(eta)}
e^{-theta d_k(gamma)}.                                   (R31)

The exact-union support theorem gives

M_{gamma eta}=0 unless eta subset gamma.

Let a=s(gamma,eta) be the fixed least cube of eta and let b be an output anchor in gamma. Then

d_k(gamma)>=dist_k(a,b).                                  (R32)

Choose nu>0 and mu_out>=0 such that

lambda_res
 :=
 theta-mu_out-nu                                          (R33)

lies in the CMP109 geometric-summability regime.

Under THIS ADDITIONAL EXPLICIT DECAY-BUDGET CONDITION,

sup_{eta contains a}
[
 e^{-mu_in d_k(eta)}
 sum_{gamma contains b:s(gamma,eta)=a}
 e^{mu_out d_k(gamma)}
 ||M_{gamma eta}(F)||
]

<=
[q(rho)/(rho-r)]
C_geo(lambda_res)
e^{-nu dist_k(a,b)}.                                      (R34)

Define

C_lat(nu)
 :=
sup_b sum_a e^{-nu dist_k(a,b)}<infinity.                 (R35)

Then weighted Schur gives

||D H_R'(F)||_{B_in,k(mu_in)->B_out,k(mu_out)}
<=
C_R'^Schur,                                                (R36)

where

C_R'^Schur
 :=
[q(rho)/(rho-r)]
C_geo(lambda_res)
C_lat(nu).                                                 (R37)

All constants are cutoff/volume/depth independent.

Important honesty point:
the existence of lambda_res in the CMP109 summability regime is NOT needed for bounded C^1 into B_out(theta). It is needed only for the stronger quasi-local Schur kernel statement. This run does not invent a numerical source threshold ensuring (R33).

## 13. YM-RPRIME-C1-1

Theorem — bounded C^1 part:
Fix Balaban source parameters beta,kappa and choose:

1. lambda_in in the CMP109 summability regime with
   mu_in=(1+beta)kappa-lambda_in>0;

2. a,theta>0 satisfying
   16a+theta<=mu_in;

3. r_*=(a/9) exp[-16a-sqrt(7)theta];

4. gamma_R'>0 small enough that
   r=C_geo(lambda_in)e^{-p_0(gamma_R')}<r_*;

5. rho=(r+r_*)/2.

Then for every RG scale k with 0<g_k<=gamma_R', the native regular/second-group Balaban cluster/log coefficient map defined by CMP122-II (1.90), (1.98) and the exact-union series crosswalk,

H_R',k^reg:
B_r(B_in,k(mu_in))
->
B_out,k(theta),

is the restriction of a holomorphic map on the r_*-ball and therefore is C^1, with the uniform bound

boxed:
sup_{||F||_in<=r}
||D H_R',k^reg(F)||
<=
C_R'^C1
:=
q(rho)/(rho-r).                                           (R38)

The constant is independent of total volume, UV cutoff and RG depth.

The actual Balaban-generated regular activity satisfies the input-ball condition because

||F^src||_in
<=
C_geo(lambda_in)e^{-p_0(g_k)}
<=r.

Status of bounded C^1 theorem:
PROVED FOR THE BALABAN NATIVE REGULAR R' CLUSTER/LOG COEFFICIENT SECTOR, with the corrected cube-or-wall compatibility constants

C_inc=9,
c_stitch=sqrt(7),

and corrected explicit radius

r_*=(a/9)e^{-16a-sqrt(7)theta}.

No unnamed source convergence radius is used.

Stronger quasi-local part:
If, in addition, there exist mu_out,nu with (R33) in the CMP109 summability regime, then the Jacobian satisfies (R34) and the Schur operator bound (R36).

Status of quasi-local Jacobian:
PROVED UNDER THE EXPLICIT ADDITIONAL DECAY-BUDGET CONDITION (R33). A numerical Balaban parameter threshold for this extra reserve is not extracted.

Non-claims:
This theorem does not cover:
- the first/boundary B' group;
- the upstream construction of F from T/E/R/B data;
- the full compressed history map;
- physical source differentiation.

## 14. Decay-reserve wording

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

## 15. Source insertion map status

Introduce a physical source space J_A and an insertion map

jmath_A:J_A->B_in,k(mu_in).

No boundedness of jmath_A is proved here.

Therefore no statement of the form

J_A in B_in

is made automatically.

The present theorem is a coefficient-space C^1 result only.

## 16. Final Iteration-9 verdict

The native Balaban regular R' cluster/log coefficient map is uniformly bounded C^1 after the exact compatibility and coordinate audits. The correct native constants are C_inc=9 and c_stitch=sqrt(7), so the explicit radius is smaller than in Iteration 8. The stronger quasi-local Schur estimate remains conditional on one additional explicit decay reserve.

The next blocker, outside this run's allowed scope, is not R' exponentiation. It is upstream:
the bounded C^1/source-local map that constructs the pre-exponentiation activity F from the earlier localized T/E/R/B history data (and, separately, the boundary B' lane).

No K_0.
No STEP_0.
